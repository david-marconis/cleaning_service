using CleaningService.Api.Models;
using Microsoft.Data.Sqlite;

namespace CleaningService.Api.Data;

public class Database
{
    private readonly ILogger<Database> logger;
    private readonly String databaseFile;

    public Database(ILogger<Database> logger, String databaseFile)
    {
        this.logger = logger;
        this.databaseFile = databaseFile;
    }

    public event EventHandler<NewAssignmentEventArgs>? NewAssignment;

    public class NewAssignmentEventArgs : EventArgs
    {
        public Assignment Assignment { get; init; }
    }

    protected virtual void OnNewAssignment(NewAssignmentEventArgs args)
    {
        logger.LogDebug($"Invoking event to {NewAssignment?.GetInvocationList()}");
        NewAssignment?.Invoke(this, args);
    }

    public IEnumerable<Assignment> GetAssignments()
    {
        return ExecuteQueryAsList("SELECT * FROM assignment", reader => new Assignment
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            User = reader.GetString(reader.GetOrdinal("user")),
            Description = reader.GetString(reader.GetOrdinal("description")),
            BidIdAssigned = reader.IsDBNull(reader.GetOrdinal("bid_id_assigned")) ? (int?)null
                : reader.GetInt32(reader.GetOrdinal("bid_id_assigned")),
            Created = reader.GetDateTime(reader.GetOrdinal("created")),
            Updated = reader.GetDateTime(reader.GetOrdinal("updated")),
        });
    }

    public Assignment InsertAssignment(Assignment assignment)
    {
        var now = DateTime.Now;
        assignment = assignment with { Created = now, Updated = now };
        using var connection = new SqliteConnection($"Data Source={databaseFile}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
        @"
                INSERT INTO assignment(user, description, created, updated)
                VALUES ($user, $description, $created, $updated)
        ";
        command.Parameters.AddWithValue("$user", assignment.User);
        command.Parameters.AddWithValue("$description", assignment.Description);
        command.Parameters.AddWithValue("$created", assignment.Created);
        command.Parameters.AddWithValue("$updated", assignment.Updated);

        command.ExecuteNonQuery();

        var idCmd = new SqliteCommand("SELECT last_insert_rowid()", connection);
        int id = Convert.ToInt32(idCmd.ExecuteScalar());
        var newAssignment = assignment with { Id = id, BidIdAssigned = null };
        OnNewAssignment(new NewAssignmentEventArgs { Assignment = newAssignment });
        return newAssignment;
    }

    public Bid InsertBid(Bid bid)
    {
        using var connection = new SqliteConnection($"Data Source={databaseFile}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
        @"
                INSERT INTO bid(assignment_id, cleaner, price, description)
                VALUES ($assignment_id, $cleaner, $price, $description)
        ";
        command.Parameters.AddWithValue("$assignment_id", bid.AssignmentId);
        command.Parameters.AddWithValue("$cleaner", bid.Cleaner);
        command.Parameters.AddWithValue("$price", bid.Price);
        command.Parameters.AddWithValue("$description", bid.Description);

        command.ExecuteNonQuery();

        var idCmd = new SqliteCommand("SELECT last_insert_rowid()", connection);
        int id = Convert.ToInt32(idCmd.ExecuteScalar());
        return bid with { Id = id };
    }

    public bool AcceptBid(int bidId)
    {
        using var connection = new SqliteConnection($"Data Source={databaseFile}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
        @"
                UPDATE assignment SET bid_id_assigned = $bid_id, updated = $updated
                WHERE bid_id_assigned IS NULL
                  AND id = (SELECT assignment_id FROM bid WHERE id = $bid_id)
        ";
        command.Parameters.AddWithValue("$bid_id", bidId);
        command.Parameters.AddWithValue("$updated", DateTime.Now);
        return command.ExecuteNonQuery() == 1;
    }

    public IEnumerable<Subscription> GetSubscriptions()
    {
        return ExecuteQueryAsList("SELECT * FROM subscription", reader => new Subscription
        {
            Name = reader.GetString(reader.GetOrdinal("name")),
            WebHook = new Uri(reader.GetString(reader.GetOrdinal("web_hook"))),
        });
    }

    public void InsertSubscription(Subscription subscription)
    {

        using var connection = new SqliteConnection($"Data Source={databaseFile}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
        @"
                INSERT INTO subscription(name, web_hook)
                VALUES ($name, $web_hook)
        ";
        command.Parameters.AddWithValue("$name", subscription.Name);
        command.Parameters.AddWithValue("$web_hook", subscription.WebHook.ToString());

        command.ExecuteNonQuery();
    }

    private IEnumerable<T> ExecuteQueryAsList<T>(
            string query,
            Func<SqliteDataReader, T> mapFunction
    )
    {
        using var connection = new SqliteConnection($"Data Source={databaseFile}");
        connection.Open();
        using var command = new SqliteCommand(query, connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            yield return mapFunction(reader);
        }
    }
}
