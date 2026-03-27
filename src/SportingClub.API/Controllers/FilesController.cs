using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportingClub.Application;

namespace SportingClub.API.Controllers;

public sealed class UploadFileRequest
{
    public string FileName { get; set; } = string.Empty;
    public string Base64Content { get; set; } = string.Empty;
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/files")]
[Authorize(Policy = "StaffOrManagerOrAdmin")]
public class FilesController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;

    public FilesController(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    [HttpPost("upload")]
    public async Task<ActionResult<object>> Upload([FromBody] UploadFileRequest request)
    {
        var path = await _fileStorageService.UploadAsync(request.FileName, request.Base64Content);
        return Ok(new { path });
    }
}
