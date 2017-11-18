﻿using System;
using System.Collections.Generic;
using RepositorioGenerico.Dictionary;
using RepositorioGenerico.Dictionary.Builders;
using RepositorioGenerico.Entities;
using RepositorioGenerico.Pattern.Contextos;
using System.Data;

namespace RepositorioGenerico.Fake.Contextos
{
	public class ContextoFake : ContextoFakeBase, IContextoFake, IContextoTransacional
	{

		public IRepositorio<TObjeto> Repositorio<TObjeto>() where TObjeto : IEntidade
		{
			var tipo = typeof(TObjeto);
			var nome = tipo.FullName;
			if (!Repositorios.ContainsKey(nome))
				CriarNovoRepositorio(nome, tipo);
			return (IRepositorio<TObjeto>)Repositorios[nome];
		}

		public IRepositorioObject Repositorio(Type tipo)
		{
			var nome = tipo.FullName;
			if (!Repositorios.ContainsKey(nome))
				CriarNovoRepositorio(nome, tipo);
			return (IRepositorioObject)Repositorios[nome];
		}

		private void CriarNovoRepositorio(string nome, Type tipo)
		{
			var tipoRepositorio = typeof(RepositorioFake<>);
			var repositorioGenerico = tipoRepositorio.MakeGenericType(tipo);
			var tipoPersistencia = typeof(PersistenciaFake<>);
			var persistenciaGenerica = tipoPersistencia.MakeGenericType(tipo);
			var dicionario = DicionarioCache.Consultar(tipo);
			var tabela = ConsultarTabelaDoBancoDeDadosVirtual(tipo);
			var persistencia = Activator.CreateInstance(persistenciaGenerica, dicionario);
			var repositorio = Activator.CreateInstance(repositorioGenerico, this, persistencia, tabela);
			Repositorios.Add(nome, repositorio);
		}

		private DataTable ConsultarTabelaDoBancoDeDadosVirtual(Type tipo, string nomeTabela = null)
		{
			var nome = nomeTabela ?? tipo.FullName;
			if (!BancoDeDadosVirtual.Tables.Contains(nome))
			{
				var dicionario = DicionarioCache.Consultar(tipo);
				var tabela = DataTableBuilder.CriarDataTable(dicionario);
				tabela.TableName = nome;
				BancoDeDadosVirtual.Tables.Add(tabela);
			}
			return BancoDeDadosVirtual.Tables[nome];
		}

		public void AdicionarRegistros<TObjeto>(IList<TObjeto> registros) where TObjeto : IEntidade
		{
			var tabela = ConsultarTabelaDoBancoDeDadosVirtual(typeof(TObjeto));
			foreach (var registro in registros)
				tabela.Rows.Add(DataTableBuilder.ConverterItemEmDataRow(tabela, registro));
		}

		public void AdicionarRegistro<TObjeto>(TObjeto registro) where TObjeto : IEntidade
		{
			var tabela = ConsultarTabelaDoBancoDeDadosVirtual(typeof(TObjeto));
			tabela.Rows.Add(DataTableBuilder.ConverterItemEmDataRow(tabela, registro));
		}

		public void DefinirResultadoProcedure<TObjeto>(string nomeProcedure, IList<TObjeto> registros) where TObjeto : IEntidade
		{
			if (string.IsNullOrEmpty(nomeProcedure))
				throw new ArgumentNullException("nomeProcedure");
			var nomeRepositorio = "__proc__" + nomeProcedure;
			var tabela = ConsultarTabelaDoBancoDeDadosVirtual(typeof(TObjeto), nomeRepositorio);
			tabela.Rows.Clear();
			foreach (var registro in registros)
				tabela.Rows.Add(DataTableBuilder.ConverterItemEmDataRow(tabela, registro));
		}

		public void DefinirResultadoScalarProcedure(string nomeProcedure, object valor)
		{
			var tabela = ConsultarTabelaDoBancoDeDadosVirtual(typeof(Procedure));
			var registro = ConsultarOuCriarRegistroTabelaProcedures(tabela, nomeProcedure);
			registro["Valor"] = valor;
		}

		private DataRow ConsultarOuCriarRegistroTabelaProcedures(DataTable tabela, string nomeProcedure)
		{
			foreach (DataRow registro in tabela.Rows)
				if (string.Equals(registro["Nome"].ToString(), nomeProcedure))
					return registro;
			var novoRegistro = tabela.NewRow();
			novoRegistro["Nome"] = nomeProcedure;
			tabela.Rows.Add(novoRegistro);
			return novoRegistro;
		}

		public void DefinirResultadoNonQueryProcedure(string nomeProcedure, int registrosAfetados)
		{
			var tabela = ConsultarTabelaDoBancoDeDadosVirtual(typeof(Procedure));
			var registro = ConsultarOuCriarRegistroTabelaProcedures(tabela, nomeProcedure);
			registro["RegistrosAfetados"] = registrosAfetados;
		}

	}
}
