using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TodoApp.Models;

namespace TodoApp.Dto;

public class UserBaseDto
{
    [Required] public string Name { get; set; }
    [Required] public DateTime DateOfBirth { get; set; }
}

public class UserCreateDto : UserBaseDto
{
    public User GetUser()
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = Name,
            DateOfBirth = DateOfBirth,
            CreateDate = DateTime.Now
        };
    }
}

public class UserGetDto : UserCreateDto
{
    public Guid Id { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public static UserGetDto GetUserDto(User user)
    {
        if (user == null) {
            throw new NotSupportedException("User can't be null");
        }

        var dto = new UserGetDto
        {
            Id = user.Id,
            Name = user.Name,
            CreateDate = user.CreateDate
        };
        if (user.UpdateDate != null) dto.UpdateDate = user.UpdateDate.Value;
        return dto;
    }
}

public class UserUpdateDto : UserBaseDto
{
    [JsonIgnore] public Guid Id { get; set; }

    public void UpdateUser(User user)
    {
        user.Name = Name;
        user.UpdateDate = DateTime.Now;
        user.DateOfBirth = DateOfBirth;
    }
}
