namespace LMS.Backend.Dtos
{
    public class UserImageDto
    {
        public int id {get; set;}
        public IFormFile profile_pic { get; set; }
    }
}