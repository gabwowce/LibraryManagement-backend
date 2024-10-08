﻿using Microsoft.AspNetCore.Mvc;
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
            return NotFound($"Member with ID {id} not found.");
        }
        return Ok(member);
    }

    [HttpGet]
    public ActionResult<IEnumerable<Member>> GetAllMembers()
    {
        var members = _memberRepository.GetAllMembers();
        if (members == null || !members.Any())
        {
            return NotFound("No members found.");
        }
        return Ok(members);
    }

    [HttpGet("admins&managers")]
    public ActionResult<IEnumerable<Member>> GetAdminAndManagerMembers()
    {
        var members = _memberRepository.GetAdminAndManagerMembers();
        if (members == null || !members.Any())
        {
            return NotFound("No admin or manager members found.");
        }
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

    [HttpPost("manager")]
    public IActionResult CreateManager([FromBody] MemberDto newManager)
    {
        if (newManager == null)
        {
            return BadRequest("Invalid manager data.");
        }

        var result = _memberRepository.CreateManager(newManager);

        if (!result)
        {
            return StatusCode(500, "An error occurred while creating the manager.");
        }

        return Ok("Manager created successfully.");
    }

    [HttpPost("admin")]
    public IActionResult CreateAdmin([FromBody] MemberDto newAdmin)
    {
        if (newAdmin == null)
        {
            return BadRequest("Invalid admin data.");
        }

        var result = _memberRepository.CreateAdmin(newAdmin);

        if (!result)
        {
            return StatusCode(500, "An error occurred while creating the admin.");
        }

        return Ok("Admin created successfully.");
    }

    [HttpDelete("{memberId}")]
    public IActionResult DeleteMember(int memberId)
    {
        var result = _memberRepository.DeleteMemberById(memberId);

        if (!result)
        {
            return BadRequest("Cannot delete the member because they have active loans.");
        }

        return Ok("Member deleted successfully.");
    }

    [HttpGet("check-member-loans/{memberId}")]
    public IActionResult CheckActiveLoans(int memberId)
    {
        var hasActiveLoans = _memberRepository.HasActiveLoans(memberId);
        return Ok(!hasActiveLoans);
    }




}
