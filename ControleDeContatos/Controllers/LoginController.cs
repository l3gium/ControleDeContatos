﻿using ControleDeContatos.Helper;
using ControleDeContatos.Models;
using ControleDeContatos.Repositorio;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeContatos.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly ISessao _sessao;
        private readonly IEmail _email;

        public LoginController(IUsuarioRepositorio usuarioRepositorio,
                               ISessao sessao,
                               IEmail email)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _sessao = sessao;
            _email = email;

        }

        public IActionResult Index()
        {
            if (_sessao.BuscarSessaoUsuario() != null) return RedirectToAction("Index", "Home");
            return View();
        }

        public IActionResult RedefinirSenha()
        {
            return View();
        }

        public IActionResult Sair()
        {
            _sessao.RemoverSessaoUsuario();

            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public IActionResult Entrar(LoginModel loginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UsuarioModel usuario = _usuarioRepositorio.BuscarPorLogin(loginModel.Login);

                    if (usuario != null)
                    {
                        if (usuario.SenhaValida(loginModel.Senha))
                        {
                            _sessao.CriarSessaoUsuario(usuario);
                            return RedirectToAction("Index", "Home");
                        }
                        TempData["MensagemErro"] = "Login ou senha inválidos";
                    }
                    TempData["MensagemErro"] = "Login ou senha inválidos";
                    
                }
                return View("Index");
            }
            catch (Exception error)
            {
                TempData["MensagemErro"] = $"Erro no login: {error.Message}";
                return RedirectToAction("Index");
            }
        }


        [HttpPost]

        public IActionResult EnviarLinkParaRedefinirSenha(RedefinirSenhaModel redefinirSenhaModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UsuarioModel usuario = _usuarioRepositorio.BuscarPorEmailELogin(redefinirSenhaModel.Email, redefinirSenhaModel.Login);

                    if (usuario != null)
                    {
                        string novaSenha = usuario.GerarNovaSenha();
                        string mensagem = $"Sua nova senha é: {novaSenha}";

                        bool emailEnviado = _email.Enviar(usuario.Email, "Sistema de Contatos - Nova Senha", mensagem);

                        if(emailEnviado) 
                        {
                            _usuarioRepositorio.Atualizar(usuario);
                            TempData["MensagemSucesso"] = $"Uma nova senha foi enviada para seu email";
                        }
                        else 
                        {
                            TempData["MensagemErro"] = "Falha ao enviar o email";
                        }

                        return RedirectToAction("Index", "Login");
                    }
                    TempData["MensagemErro"] = $"Falha ao enviar o email. Verifique os dados informados";
                }
                return View("Index");
            }
            catch (Exception error)
            {
                TempData["MensagemErro"] = $"Erro ao redefinir senha: {error.Message}";
                return RedirectToAction("Index");
            }

            //try
            //{
            //    UsuarioModel usuario = _usuarioRepositorio.BuscarPorEmailELogin(redefinirSenhaModel.Email, redefinirSenhaModel.Login);
            //    if (usuario != null)
            //    {
            //        string novaSenha = usuario.GerarNovaSenha();
            //        string mensagem = $"Sua nova senha é: {novaSenha}";

            //        bool emailEnviado = _email.Enviar(usuario.Email, "Sistema de Contatos - Nova Senha", mensagem);

            //        if(emailEnviado) 
            //        {
            //            _usuarioRepositorio.Atualizar(usuario);
            //            TempData["MensagemSucesso"] = "Uma nova senha foi enviada ao seu email";
            //        }
            //        else
            //        {
            //            TempData["MensagemErro"] = "Erro no envio do email. Tente novamente";
            //        }

            //        return RedirectToAction("Index", "Login");
            //    }

            //    TempData["MensagemErro"] = "Erro na redefinição de senha. Verifique os dados informados";
            //    return View("Index");
            //}
            //catch (Exception error)
            //{
            //    TempData["MensagemErro"] = $"Erro na redefinição de senha: {error.Message}";
            //    return RedirectToAction("Index");
            //}
        }
    }
}