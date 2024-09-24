using LibraryManagement.Models;
using System.Collections.Generic;

namespace LibraryManagement.Interfaces
{
    public interface IMemberRepository
    {
        Member GetMemberById(int id);
        MemberDto GetByUsername(string username);
        IEnumerable<Member> GetAllMembers();
        IEnumerable<Member> GetAdminAndManagerMembers();
        bool UpdateMember(MemberDto member, int id);
        bool CreateMember(MemberDto member);
        bool CreateManager(MemberDto member);
        bool CreateAdmin(MemberDto member);
        bool HasActiveLoans(int memberId);
        bool DeleteMemberById(int memberId);

    }
}
