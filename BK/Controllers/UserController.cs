using BK.Models;
using BK.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BK.Controllers
{
    [Route(template: "api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetUser(int id)
        {
            var user = _userRepository.GetUserById(id);

            if (user == null)
                return NotFound();
            else
                return Ok(user);
        }

        [HttpPost]
        public ActionResult<User> CreateUser([FromBody] User user)
        {
            if (user is null)
                return BadRequest("объект пользователя пришел пустым");

            if (String.IsNullOrEmpty(user.Email) ||
                String.IsNullOrEmpty(user.Login))
                return BadRequest("пустое поле почты или логина");

            var newUser = _userRepository.AddUser(user);

            return CreatedAtAction(nameof(CreateUser),
                new { Id = newUser.Id }, newUser);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            if (_userRepository.DeleteUser(id))
                return NoContent();
            else
                return NotFound();
        }

        [HttpPut("{id}")]
        public ActionResult<User> UpdateUser(int id, [FromBody] User user)
        {
            if (user is null)
                return BadRequest("некорретные данные для обновления");
            if (id != user.Id)
                return BadRequest("несовпадения по id");

            var updateUser = _userRepository.UpdateUser(id, user);
            return Ok(updateUser);

        }
    }

}

