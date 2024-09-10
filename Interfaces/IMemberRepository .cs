using LibraryManagement.Models;
using System.Collections.Generic;

namespace LibraryManagement.Interfaces
{
    public interface IMemberRepository
    {
        Member GetMemberById(int id);
        IEnumerable<Member> GetAllMembers();
    }
}
