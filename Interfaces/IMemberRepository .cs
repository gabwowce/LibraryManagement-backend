using LibraryManagement.Models;
using System.Collections.Generic;

namespace LibraryManagement.Interfaces
{
    public interface IMemberRepository
    {
        Member GetMemberById(int id);
        IEnumerable<Member> GetAllMembers();
        bool UpdateMember(MemberDto member, int id);
        bool CreateMember(MemberDto member);
        bool HasActiveLoans(int memberId);
        bool DeleteMemberById(int memberId);

    }
}
