using LoginService.Mapper;
using Microsoft.AspNetCore.Mvc;

namespace LoginService.Controller;

/// <summary>
/// @author Xioa
/// @date  2024年12月4日
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class CreateBaseController : ControllerBase
{
    [HttpGet]
    public async Task<bool> CreateBase()
    {
        await using var _db = new DbDataContext();
        //await _db.Database.EnsureDeletedAsync();
        return await _db.Database.EnsureCreatedAsync();
    }
}