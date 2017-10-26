﻿using System;
using System.Data;
using System.Globalization;
using RepositorioGenerico.Exceptions;

namespace RepositorioGenerico.Fake
{
	public class DbCommandFake : IDbCommand
	{

		private readonly DataSet _bancoDeDadosVirtual;

		private readonly IDataParameterCollection _parameters;

		public IDbConnection Connection { get; set; }

		public IDbTransaction Transaction { get; set; }

		public string CommandText { get; set; }

		public int CommandTimeout { get; set; }

		public CommandType CommandType { get; set; }

		public IDataParameterCollection Parameters
		{
			get { return _parameters; }
		}

		public UpdateRowSource UpdatedRowSource { get; set; }

		public DbCommandFake(DataSet bancoDeDadosVirtual)
		{
			_parameters = new DataParametersCollectionFake();
			_bancoDeDadosVirtual = bancoDeDadosVirtual;
		}

		public void Prepare()
		{
			throw new NotImplementedException();
		}

		public void Cancel()
		{
			throw new NotImplementedException();
		}

		public IDbDataParameter CreateParameter()
		{
			return new DbDataParameterFake();
		}

		public int ExecuteNonQuery()
		{
			throw new NotImplementedException();
		}

		public IDataReader ExecuteReader()
		{
			var view = ConsultarTabelaComDadosVirtuais().DefaultView;
            var sql = CommandText.ToLower();
            var indiceTop = sql.IndexOf("select top ", StringComparison.Ordinal);
            var fimTop = sql.IndexOf(" ", 11, StringComparison.Ordinal);
            var top = ((indiceTop == 0) && (fimTop > 0))
                ? Convert.ToInt32(sql.Substring(11, fimTop - 11))
                : 0;
            var indiceWhere = sql.IndexOf("where", StringComparison.Ordinal);
			var condicao = (indiceWhere > 0)
				? ConsultarCondicaoParaFiltragem(CommandText.Substring(indiceWhere + 5))
				: string.Empty;
            var indiceOrderby = sql.IndexOf("order by", StringComparison.Ordinal);
            if ((indiceOrderby > 0) && (indiceOrderby > indiceWhere))
            {
                condicao = condicao.Substring(0, indiceOrderby - indiceWhere - 8 + 1 - 5 + 1);
                view.Sort = CommandText.Substring(indiceOrderby + 8);
            }
			if (!string.IsNullOrEmpty(condicao))
				view.RowFilter = condicao;
			return new DataReaderFake(view, top);
		}

		private DataTable ConsultarTabelaComDadosVirtuais()
		{
			if (_bancoDeDadosVirtual.Tables.Count == 1)
				return _bancoDeDadosVirtual.Tables[0];
			var sql = CommandText.ToLower();
			var from = sql.IndexOf("from", StringComparison.Ordinal);
			if (from == -1)
				throw new NaoFoiPossivelDeterminarONomeDaTabelaFakeException();
			sql = sql.Substring(from + 4).Trim();
			var corte = sql.IndexOf(sql.StartsWith("[") ? "]" : " ", StringComparison.Ordinal);
			if (corte == -1)
				throw new NaoFoiPossivelDeterminarONomeDaTabelaFakeException();
			var nome = sql.Substring(0, corte)
				.Replace("[", string.Empty)
				.Replace("]", string.Empty);
			if (!_bancoDeDadosVirtual.Tables.Contains(nome))
				throw new NaoFoiPossivelDeterminarONomeDaTabelaFakeException();
			return _bancoDeDadosVirtual.Tables[nome];
		}

		private string ConsultarCondicaoParaFiltragem(string condicao)
		{
			foreach (IDbDataParameter parametro in Parameters)
				condicao = condicao.Replace(parametro.ParameterName, ConverterParametroEmConstange(parametro));
			return condicao;
		}

		private string ConverterParametroEmConstange(IDataParameter parametro)
		{
			if ((parametro.Value == DBNull.Value) || (parametro.Value == null))
				return "NULL";

			switch (parametro.DbType)
			{
				case DbType.Boolean:
					return Convert.ToBoolean(parametro.Value)
						? "True"
						: "False";

				case DbType.Date:
				case DbType.DateTime:
				case DbType.DateTime2:
				case DbType.DateTimeOffset:
					return "'" + Convert.ToDateTime(parametro.Value).ToString("yyyy-MM-dd HH:mm:ss") + "'";

				case DbType.Currency:
				case DbType.Decimal:
				case DbType.Double:

				case DbType.Int16:
				case DbType.Int32:
				case DbType.Int64:
				case DbType.Single:

				case DbType.UInt16:
				case DbType.UInt32:
				case DbType.UInt64:
				case DbType.VarNumeric:
					return Convert.ToDouble(parametro.Value).ToString(CultureInfo.InvariantCulture)
						.Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, ".");

				default:
					return "'" + parametro.Value.ToString().Replace("'", "''") + "'";

			}
		}

		public IDataReader ExecuteReader(CommandBehavior behavior)
		{
			return ExecuteReader();
		}

		public object ExecuteScalar()
		{
			using (var reader = ExecuteReader())
				if (reader.Read())
					return reader[0];
			return null;
		}

		public void Dispose()
		{

		}

	}
}
