using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Leo.Poulet.FeatureMatching;
using Leo.Poulet.FeatureMatching;

namespace Leo.Poulet.FeatureMatching.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                System.Console.WriteLine("Usage: dotnet run <objectImagePath> <scenesDirectoryPath>");
                return;
            }

            var objectImagePath = args[0];
            var scenesDirectoryPath = args[1];

            if (!File.Exists(objectImagePath))
            {
                System.Console.WriteLine($"Error: File '{objectImagePath}' not found.");
                return;
            }

            if (!Directory.Exists(scenesDirectoryPath))
            {
                System.Console.WriteLine($"Error: Directory '{scenesDirectoryPath}' not found.");
                return;
            }

            var objectImageData = await File.ReadAllBytesAsync(objectImagePath);
            var sceneImagePaths = Directory.GetFiles(scenesDirectoryPath);
            var sceneImageData = new List<byte[]>();

            foreach (var sceneImagePath in sceneImagePaths)
            {
                var imageData = await File.ReadAllBytesAsync(sceneImagePath);
                sceneImageData.Add(imageData);
            }

            var objectDetection = new ObjectDetection();
            var detectionResults = await objectDetection.DetectObjectInScenesAsync(objectImageData, sceneImageData);

            foreach (var result in detectionResults)
            {
                System.Console.WriteLine($"Points: {JsonSerializer.Serialize(result.Points)}");
            }
        }
    }
}