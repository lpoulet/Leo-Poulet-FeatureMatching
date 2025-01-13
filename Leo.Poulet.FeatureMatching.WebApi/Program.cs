using Leo.Poulet.FeatureMatching;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();
app.MapPost("/FeatureMatching", async ([FromForm] IFormFileCollection files) =>
{
    // Vérification des fichiers envoyés
    if (files.Count != 2)
    {
        return Results.BadRequest("Two files are required: one object image and one scene image.");
    }

    // Lecture du fichier image de l'objet
    using var objectSourceStream = files[0].OpenReadStream();
    using var objectMemoryStream = new MemoryStream();
    await objectSourceStream.CopyToAsync(objectMemoryStream);
    var imageObjectData = objectMemoryStream.ToArray();

    // Lecture du fichier image de la scène
    using var sceneSourceStream = files[1].OpenReadStream();
    using var sceneMemoryStream = new MemoryStream();
    await sceneSourceStream.CopyToAsync(sceneMemoryStream);
    var imageSceneData = sceneMemoryStream.ToArray();

    // Utilisation de votre librairie pour détecter l'objet
    var objectDetection = new ObjectDetection();
    var detectionResults = await objectDetection.DetectObjectInScenesAsync(imageObjectData, new[] { imageSceneData });

    // Retourner la première image détectée avec les zones marquées
    var resultImageData = detectionResults[0].ImageData;
    return Results.File(resultImageData, "image/png");

}).DisableAntiforgery();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
