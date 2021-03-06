﻿using RepositorioGenerico.Entities.Mapas;

namespace RepositorioGenerico.Test.Objetos
{
	public class FilhoMapeadoDoObjetoMapeadoDeTestesMap : MapaEntidade<FilhoMapeadoDoObjetoMapeadoDeTestes>
	{
		public override void Referenciar(IMapa<FilhoMapeadoDoObjetoMapeadoDeTestes> builder)
		{
			builder.Tabela<FilhoDoObjetoDeTestes>()
				.Campo(c => c.Id).Propriedade(p => p.MapeadoComCodigoFilho)
				.Campo(c => c.Nome).Propriedade(p => p.MapeadoComNomeFilho)
				.Campo(c => c.IdPai).Propriedade(p => p.MapeadoComCodigoPai)
				.Campo(c => c.Pai).Propriedade(p => p.MapeadoComPai);
		}
	}
}
