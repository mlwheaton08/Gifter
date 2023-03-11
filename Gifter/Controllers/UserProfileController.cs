using Gifter.Models;
using Gifter.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gifter.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileRepository _userProfileRepository;
    public UserProfileController(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_userProfileRepository.GetAll());
    }

    [HttpGet("GetWithPosts")]
    public IActionResult GetWithPosts()
    {
        return Ok(_userProfileRepository.GetAllWithPosts());
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var profile = _userProfileRepository.GetById(id);
        if (profile == null)
        {
            return NotFound();
        }
        return Ok(profile);
    }

    [HttpGet("GetWithPosts/{id}")]
    public IActionResult GetByIdWithPosts(int id)
    {
        var profile = _userProfileRepository.GetByIdWithPosts(id);
        if (profile == null)
        {
            return NotFound();
        }
        return Ok(profile);
    }

    [HttpPost]
    public IActionResult Post(UserProfile profile)
    {
        _userProfileRepository.Add(profile);
        return CreatedAtAction("Get", new { id = profile.Id }, profile);
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, UserProfile profile)
    {
        if (id != profile.Id)
        {
            return BadRequest();
        }

        _userProfileRepository.Update(profile);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _userProfileRepository.Delete(id);
        return NoContent();
    }
}