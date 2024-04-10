using AlgorithmAlley.DTO;
using AlgorithmAlley.Models;

namespace AlgorithmAlley.Services.IServices
{
    public interface IUserServices
    {
        Task<APIResponses<UserInformationDTO>> SignUpAsync(UserInformationDTO userInformationDTO);
        Task<APIResponses<string>> LoginAsync(string email,string password);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        Task<bool> IsEmailUniqueAsync(string email);
    }
}
