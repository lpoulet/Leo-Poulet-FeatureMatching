using System.Collections.Generic; 
using System.IO; 
using System.Reflection; 
using System.Text.Json; 
using System.Threading.Tasks; 
using Xunit; 
 
 
namespace Leo.Poulet.FeatureMatching.Tests; 
 
public class FeatureMatchingUnitTest 
{ 
    [Fact] 
    public async Task ObjectShouldBeDetectedCorrectly() 
    { 
        var executingPath = GetExecutingPath(); 
        var imageScenesData = new List<byte[]>(); 
        foreach (var imagePath in Directory.EnumerateFiles(Path.Combine(executingPath, 
                     "Scenes"))) 
        { 
            var imageBytes = await File.ReadAllBytesAsync(imagePath); 
            imageScenesData.Add(imageBytes); 
        } 
 
        var objectImageData = await File.ReadAllBytesAsync(Path.Combine(executingPath, 
            "Guillaume-Chervet-object.png")); 
 
        var detectObjectInScenesResults = await new 
            ObjectDetection().DetectObjectInScenesAsync(objectImageData, imageScenesData); 
 
        
        Assert.Equal("[{\"X\":117,\"Y\":160},{\"X\":89,\"Y\":272},{\"X\":267,\"Y\":297},{\"X\":282,\"Y\":176}]",JsonSerializer.Serialize(detectObjectInScenesResults[0].Points)); 
        
        Assert.Equal("[{\"X\":117,\"Y\":160},{\"X\":89,\"Y\":272},{\"X\":267,\"Y\":297},{\"X\":282,\"Y\":176}]",JsonSerializer.Serialize(detectObjectInScenesResults[1].Points)); 
    } 
 
    private static string GetExecutingPath() 
    { 
        var executingAssemblyPath = Assembly.GetExecutingAssembly().Location; 
        var executingPath = Path.GetDirectoryName(executingAssemblyPath); 
        return executingPath; 
    } 
}