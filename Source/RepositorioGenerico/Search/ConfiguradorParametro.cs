﻿using System;
using System.Data;
using RepositorioGenerico.Pattern.Buscadores;

namespace RepositorioGenerico.Search
{
	public class ConfiguradorParametro: IConfiguracaoParametro
	{

		private readonly Configurador _configurador;
		private readonly string _nome;

		public ConfiguradorParametro(Configurador configurador, string nome)
		{
			_configurador = configurador;
			_nome = nome;
		}

		public IConfiguracao Valor(string valor, int tamanhoMaximo = 0)
		{
			return DefinirParametro(DbType.String, valor, tamanhoMaximo);
		}

		private IConfiguracao DefinirParametro(DbType tipo, object valor, int tamanhoMaximo = 0)
		{
			var novo = !_configurador.Comando.Parameters.Contains("@" + _nome);
			var parametro = (novo)
				? _configurador.Comando.CreateParameter()
				: (IDbDataParameter)_configurador.Comando.Parameters["@" + _nome];
			parametro.ParameterName = "@" + _nome;
			parametro.DbType = tipo;
			parametro.Value = valor ?? DBNull.Value;
			if (tamanhoMaximo > 0)
				parametro.Size = tamanhoMaximo;
			if (novo)
				_configurador.Comando.Parameters.Add(parametro);
			return _configurador;
		}

		public IConfiguracao Valor(int valor)
		{
			return DefinirParametro(DbType.Int32, valor);
		}

		public IConfiguracao Valor(int? valor)
		{
			return DefinirParametro(DbType.Int32, valor);
		}

		public IConfiguracao Valor(double valor)
		{
			return DefinirParametro(DbType.Double, valor);
		}

		public IConfiguracao Valor(double? valor)
		{
			return DefinirParametro(DbType.Double, valor);
		}

		public IConfiguracao Valor(decimal valor)
		{
			return DefinirParametro(DbType.Decimal, valor);
		}

		public IConfiguracao Valor(decimal? valor)
		{
			return DefinirParametro(DbType.Decimal, valor);
		}

		public IConfiguracao Valor(bool valor)
		{
			return DefinirParametro(DbType.Boolean, valor);
		}

		public IConfiguracao Valor(DateTime valor)
		{
			return DefinirParametro(DbType.DateTime, valor);
		}

		public IConfiguracao Valor(DateTime? valor)
		{
			return DefinirParametro(DbType.DateTime, valor);
		}

		public IConfiguracao Nulo()
		{
			return DefinirParametro(DbType.Object, DBNull.Value);
		}

		public IConfiguracao Tipo(DbType tipo, object valor)
		{
			return DefinirParametro(tipo, valor);
		}
	}
}
