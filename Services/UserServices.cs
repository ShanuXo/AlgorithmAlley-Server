using AlgorithmAlley.DTO;
using AlgorithmAlley.Models;
using AlgorithmAlley.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Net;

namespace AlgorithmAlley.Services
{
    public class UserServices : IUserServices
    {
        private readonly IMongoCollection<UserInformation> _usersInformation;

        public UserServices(IMongoDatabase database)
        {
            _usersInformation = database.GetCollection<UserInformation>("AlgorithmAlley");

            // Create a unique index on the Email field to enforce uniqueness
            var emailIndexDefinition = Builders<UserInformation>.IndexKeys.Ascending(u => u.Email);
            var uniqueEmailIndexOptions = new CreateIndexOptions { Unique = true };
            var emailIndexModel = new CreateIndexModel<UserInformation>(emailIndexDefinition, uniqueEmailIndexOptions);
            _usersInformation.Indexes.CreateOne(emailIndexModel);
        }
        public async Task<APIResponses<UserInformationDTO>> SignUpAsync(UserInformationDTO userInformationDTO)
        {
            try
            {
                if (await IsEmailUniqueAsync(userInformationDTO.Email))
                {
                    // Hash the password
                    string hashedPassword = HashPassword(userInformationDTO.Password);

                    // Create a new user
                    var newUser = new UserInformation
                    {
                        FirstName = userInformationDTO.FirstName,
                        LastName = userInformationDTO.LastName,
                        Email = userInformationDTO.Email,
                        Password = hashedPassword,
                        ConfirmPassword = hashedPassword,
                        PhoneNumber = userInformationDTO.PhoneNumber
                    };

                    await _usersInformation.InsertOneAsync(newUser);

                    return new APIResponses<UserInformationDTO>
                    {
                        StatusCode = HttpStatusCode.Created,
                        IsSuccess = true,
                        Result = userInformationDTO
                    };
                }
                else
                {
                    return new APIResponses<UserInformationDTO>
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        IsSuccess = false,
                        ErrorMessage = new List<string> { "Email address already exists." }
                    };
                }
            }
            catch (Exception ex)
            {
                return new APIResponses<UserInformationDTO>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessage = new List<string> { ex.Message }
                };
            }
        }
        public async Task<APIResponses<string>> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _usersInformation.Find(u => u.Email == email)
                                .Project(u => new { u.Email, u.Password })
                                .FirstOrDefaultAsync();

                if (user == null)
                {
                    return new APIResponses<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        IsSuccess = false,
                        ErrorMessage = new List<string> { "User not found." }
                    };
                }

                if (!VerifyPassword(password, user.Password))
                {
                    return new APIResponses<string>
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        IsSuccess = false,
                        ErrorMessage = new List<string> { "Incorrect password." }
                    };
                }

                return new APIResponses<string>
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Result = "Login successful."
                };
            }
            catch (Exception ex)
            {
                return new APIResponses<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessage = new List<string> { ex.Message }
                };
            }
        }
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password,hashedPassword);
        }
        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            var existingUser = await _usersInformation.Find(u => u.Email == email).FirstOrDefaultAsync();
            return existingUser == null;
        }
    }
}
