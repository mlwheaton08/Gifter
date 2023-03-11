using Gifter.Models;

namespace Gifter.Repositories
{
    public interface IUserProfileRepository
    {
        void Add(UserProfile profile);
        void Delete(int id);
        List<UserProfile> GetAll();
        List<UserProfile> GetAllWithPosts();
        UserProfile GetById(int id);
        UserProfile GetByIdWithPosts(int id);
        void Update(UserProfile profile);
    }
}