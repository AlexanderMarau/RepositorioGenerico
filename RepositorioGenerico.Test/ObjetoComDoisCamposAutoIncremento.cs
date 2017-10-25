﻿using RepositorioGenerico.Entities;
using RepositorioGenerico.Entities.Anotacoes;

namespace RepositorioGenerico.Test
{
	public class ObjetoComDoisCamposAutoIncremento : Entidade
	{

		[Chave]
		[Obrigatorio]
		[AutoIncremento(Incremento.Identity)]
		[Coluna(Ordem = 0, NomeDoTipo = "int")]
		public int CodigoUm { get; set; }

		[Chave]
		[Obrigatorio]
		[AutoIncremento(Incremento.Identity)]
		[Coluna(Ordem = 1, NomeDoTipo = "int")]
		public int CodigoDois { get; set; }


		[Chave]
		[Obrigatorio]
		[Coluna(Ordem = 2, NomeDoTipo = "varchar")]
		public string Nome { get; set; }

	}
}
