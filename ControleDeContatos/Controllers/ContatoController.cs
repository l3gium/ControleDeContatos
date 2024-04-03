using ControleDeContatos.Filters;
using ControleDeContatos.Helper;
using ControleDeContatos.Models;
using ControleDeContatos.Repositorio;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeContatos.Controllers
{
    [PaginaParaUsuarioLogado]
    public class ContatoController : Controller
    {
        private readonly IContatoRepositorio _contatoRepositorio;
        private readonly ISessao _sessao;
        public ContatoController(IContatoRepositorio contatoReporitorio,
                                 ISessao sessao)
        {
            _contatoRepositorio = contatoReporitorio;
            _sessao = sessao;
        }

        public IActionResult Index()
        {
            UsuarioModel usuarioLogado = _sessao.BuscarSessaoUsuario();
            List<ContatoModel> contatos = _contatoRepositorio.BuscarTodos(usuarioLogado.Id);

            return View(contatos);
        }

        public IActionResult Criar()
        {
            return View();
        }

        public IActionResult Editar(int id)
        {
            ContatoModel contato = _contatoRepositorio.ListarPorId(id);
            return View(contato);
        }

        public IActionResult ApagarConfirmacao(int id)
        {
            ContatoModel contato = _contatoRepositorio.ListarPorId(id);
            return View(contato);
        }

        public IActionResult Apagar(int id)
        {
            try
            {
                bool apagado = _contatoRepositorio.Apagar(id);

                if (apagado)
                {
                    TempData["MensagemSucesso"] = "Contato apagado com sucesso!";
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao apagar contato";
                }
                return RedirectToAction("Index");
            }
            catch (Exception erro) 
            {
                TempData["MensagemErro"] = $"Erro ao apagar contato: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]

        //public IActionResult Criar(ContatoModel contato)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            UsuarioModel usuarioLogado = _sessao.BuscarSessaoUsuario();
        //            contato.UsuarioId = usuarioLogado.Id;

        //            contato = _contatoRepositorio.Adicionar(contato);
        //            TempData["MensagemSucesso"] = "Contato cadastrado com sucesso!";
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    catch(System.Exception erro) 
        //    {
        //        TempData["MensagemErro"] = $"Erro no cadastro do usuário: {erro.Message}";
        //        return RedirectToAction("Index");
        //    }
        //    return View(contato);
        //}
        public IActionResult Criar(ContatoModel contato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UsuarioModel usuarioLogado = _sessao.BuscarSessaoUsuario();
                    contato.UsuarioId = usuarioLogado.Id;

                    contato = _contatoRepositorio.Adicionar(contato);

                    TempData["MensagemSucesso"] = "Contato cadastrado com sucesso!";
                    return RedirectToAction("Index");
                }
                return View(contato);
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não conseguimos cadastrar seu contato, tente novamante, detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]

        public IActionResult Alterar(ContatoModel contato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UsuarioModel usuarioLogado = _sessao.BuscarSessaoUsuario();
                    contato.UsuarioId = usuarioLogado.Id;

                    contato = _contatoRepositorio.Atualizar(contato);
                    TempData["MensagemSucesso"] = "Contato atualizado com sucesso!";
                    return RedirectToAction("Index");
                }
            }
            catch(System.Exception erro) 
            {
                TempData["MensagemErro"] = $"Erro na atualização do usuário: {erro.Message}";
                return RedirectToAction("Index");
            }
            return View(contato);
        }
    }
}
