using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Interfaces;
using LibraryManagement.Models;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly IMemberRepository _memberRepository;

    public MembersController(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    [HttpGet("{id}")]
    public ActionResult<Member> GetMemberById(int id)
    {
        var member = _memberRepository.GetMemberById(id);
        if (member == null)
        {
            return NotFound();
        }
        return Ok(member);
    }

    [HttpGet]
    public ActionResult<IEnumerable<Member>> GetAllMembers()
    {
        var members = _memberRepository.GetAllMembers();
        return Ok(members);
    }

    [HttpPut("member/{id}")]
    public IActionResult UpdateMember(int id, [FromBody] MemberDto updatedMember)
    {
        if (updatedMember == null)
        {
            return BadRequest("Invalid member data.");
        }

        var result = _memberRepository.UpdateMember(updatedMember, id);

        if (!result)
        {
            return StatusCode(500, "An error occurred while updating the member.");
        }

        return Ok("Member updated successfully.");
    }


    [HttpPost("member")]
    public IActionResult CreateMember([FromBody] MemberDto newMember)
    {
        if (newMember == null)
        {
            return BadRequest("Invalid member data.");
        }

        var result = _memberRepository.CreateMember(newMember);

        if (!result)
        {
            return StatusCode(500, "An error occurred while creating the member.");
        }

        return Ok("Member created successfully.");
    }



}
