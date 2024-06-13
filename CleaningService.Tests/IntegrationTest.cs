using System.Text;
using System.Text.Json;
using CleaningService.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;

namespace CleaningService.Tests;

public class IntegrationTest : IClassFixture<WebApplicationFactory<CleaningService.Api.Program>>
{
    private readonly HttpClient client;

    public IntegrationTest(WebApplicationFactory<CleaningService.Api.Program> factory)
    {
        client = factory.CreateClient();
        SetupDatabase();
    }

    [Fact]
    public async Task TestAssignmentsAndBids()
    {
        var jsonOpts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // Test post assignment
        var assignment = new Assignment { User = "user1", Description = "", Created = DateTime.Now, Updated = DateTime.Now };
        var postResponse = await PostAsyncJson("/api/assignment", assignment);
        postResponse.EnsureSuccessStatusCode();

        // Test list asignments
        var listResponse = await client.GetAsync("/api/assignment/list");
        listResponse.EnsureSuccessStatusCode();
        var listResponseContent = await listResponse.Content.ReadAsStringAsync();
        var assignments = JsonSerializer.Deserialize<List<Assignment>>(listResponseContent, jsonOpts)!;

        Assert.Single(assignments);
        var actualAssignment = assignments[0];
        Assert.Equal(assignment.User, actualAssignment.User);

        // Test bid for assignment
        var bid = new Bid { AssignmentId = actualAssignment.Id!.Value, Cleaner = "Cleaner", Description = "", Price = 0 };
        var bidPostResponse = await PostAsyncJson("/api/bid", bid);
        bidPostResponse.EnsureSuccessStatusCode();
        var bidJson = await bidPostResponse.Content.ReadAsStringAsync();
        var actualBid = JsonSerializer.Deserialize<Bid>(bidJson, jsonOpts);

        // Test accept bid
        var putResponse = await client.PutAsync($"/api/bid/{actualBid.Id!.Value}", null);
        putResponse.EnsureSuccessStatusCode();

        // Ensure bid is assignmed
        var updatedAssignmentsResponse = await client.GetAsync("/api/assignment/list");
        updatedAssignmentsResponse.EnsureSuccessStatusCode();
        var updatedAssignmentsJson = await updatedAssignmentsResponse.Content.ReadAsStringAsync();
        var updatedAssignments = JsonSerializer.Deserialize<List<Assignment>>(updatedAssignmentsJson, jsonOpts)!;
        Assert.Single(updatedAssignments);
        Assert.Equal(actualBid.Id, updatedAssignments[0].BidIdAssigned);
    }

    private async Task<HttpResponseMessage> PostAsyncJson<T>(string requestUri, T value)
    {
        var json = JsonSerializer.Serialize(value);
        var postContent = new StringContent(json, Encoding.UTF8, "application/json");
        return await client.PostAsync(requestUri, postContent);
    }

    private void SetupDatabase()
    {
        File.Delete("cleaning_service.db");
        using var connection = new SqliteConnection($"Data Source=cleaning_service.db");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = System.IO.File.ReadAllText("../../../../CleaningService.Api/database_schema.sql");
        command.ExecuteNonQuery();
    }
}

