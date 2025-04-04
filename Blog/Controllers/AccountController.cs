using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace Blog.Controllers;

[ApiController]
public class AccountController : ControllerBase
{
    [HttpPost("v1/accounts/")]
    public async Task<IActionResult> Post(
        [FromBody] RegisterViewModel model,
        [FromServices]EmailService emailService,
        [FromServices]BlogDataContext context) 
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            Bio = model.Bio,
            Slug = model.Email.Replace("@", "-").Replace(".", "-"),
            Image = model.Image
        };

        var password = PasswordGenerator.Generate(20);
        user.PasswordHash = PasswordHasher.Hash(password);

        try
        {
            emailService.Send(
                user.Name,
                user.Email,
                "Bem-vindo ao Blog",
                $"Sua senha é <strong>{password}</strong>"
            );

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            //emailService.Send(
            //    user.Name,
            //    user.Email,
            //    "Bem-vindo ao Blog",
            //    $"Sua senha é <strong>{password}</strong>"
            //);
            return Ok(new ResultViewModel<dynamic>(new
            { 
                user = user.Email, password
            }));
        }
        catch(DbUpdateException)
        {
            return StatusCode(400, new ResultViewModel<string>("E-mail já cadastrado"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("Erro interno"));
        }
    }

    [HttpPost("v1/accounts/login")]
    public async Task <IActionResult> Login(
        [FromBody] LoginViewModel model,
        [FromServices]TokenService tokenService,
        [FromServices]BlogDataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

        var user = await context
            .Users
            .AsNoTracking()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Email == model.Email);

        if (user == null)
            return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

        if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
            return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

        try
        {
            var token = tokenService.GenerateToken(user);
            return Ok(new ResultViewModel<string>(token, null));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
        }
    }

}

