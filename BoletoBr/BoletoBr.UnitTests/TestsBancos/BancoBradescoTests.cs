﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoletoBr.Bancos.Bradesco;
using BoletoBr.Dominio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoletoBr.UnitTests.Tests.Bancos
{
    [TestClass]
    public class BancoBradescoTests
    {
        [TestMethod]
        public void FormataNumeroDocumentoHsbc()
        {
            Boleto boleto = new Boleto();
            BancoBradesco bradesco = new BancoBradesco();

            string numeroDocumento = "123";

            const string valorEsperadoCnab240 = "000000000000123";
            const string valorEsperadoCnab400 = "0000000123";

            boleto.NumeroDocumento = numeroDocumento;
            boleto.TipoArquivo = TipoArquivo.Cnab400;
            bradesco.FormataNumeroDocumento(boleto);

            string numeroDocumentoFormatado = boleto.NumeroDocumento;

            Assert.AreEqual(valorEsperadoCnab400, numeroDocumentoFormatado);
        }

        [TestMethod]
        public void CalculoNossoNumeroCarteira06DocumentacaoBradesco()
        {
            var banco = Fabricas.BancoFactory.ObterBanco("237", "2");

            var contaBancariaCedente = new ContaBancaria("2374", "4", "0165199", "4");

            var cedente = new Cedente("9999999", 0, "99.999.999/9999-99", "Razão Social X", contaBancariaCedente, null);

            var sacado = new Sacado("Sacado Fulano de Tal", "999.999.999-99", new Endereco()
            {
                TipoLogradouro = "R",
                Logradouro = "1",
                Bairro = "Bairro X",
                Cidade = "Cidade X",
                SiglaUf = "XX",
                Cep = "12345-000",
                Complemento = "Comp X",
                Numero = "9"
            });

            var boleto = new Boleto(cedente, sacado, banco.GetCarteiraCobrancaPorCodigo("06"));
            boleto.NumeroDocumento = "41636135093";
            boleto.ValorBoleto = Convert.ToDecimal(221.40);
            boleto.SequencialNossoNumero = "41636135093";
            boleto.DataVencimento = new DateTime(2014, 07, 10);

            banco.FormataNossoNumero(boleto);

            Assert.AreEqual("06/41636135093-P", boleto.NossoNumeroFormatado);
        }

        [TestMethod]
        public void CalculoLinhaDigitavelCarteira06DocumentacaoBradesco()
        {
            /* 
             * Teste baseado em um boleto apresentado na documentação oficial do Banco da Amazônia versão 1
             * Manual de Cobrança Registrada Simples
             */
            var banco = Fabricas.BancoFactory.ObterBanco("237", "2");

            var contaBancariaCedente = new ContaBancaria("1234", "8", "12345", "6");

            var cedente = new Cedente("99999", "1", 0, "99.999.999/9999-99", "Razao Social X", contaBancariaCedente, null);

            var sacado = new Sacado("Sacado Fulano de Tal", "99.999.999/9999-99", new Endereco()
            {
                TipoLogradouro = "R",
                Logradouro = "1",
                Bairro = "Bairro X",
                Cidade = "Cidade X",
                SiglaUf = "XX",
                Cep = "12345-000",
                Complemento = "Comp X",
                Numero = "9"
            });

            var boleto = new Boleto(cedente, sacado, banco.GetCarteiraCobrancaPorCodigo("06"));
            boleto.NumeroDocumento = "3242";
            boleto.ValorBoleto = Convert.ToDecimal(275);
            boleto.SequencialNossoNumero = "3242";
            boleto.DataVencimento = new DateTime(2014, 08, 04);

            banco.FormatarBoleto(boleto);

            const string valorEsperado = "00390.07802 00100.000009  00000.012385 8 39160000001556";
            Assert.AreEqual(valorEsperado.Length, boleto.LinhaDigitavelBoleto.Length);
            Assert.AreEqual(valorEsperado, boleto.LinhaDigitavelBoleto);
        }

        [TestMethod]
        public void CalculoCodigoBarrasCarteira06DocumentacaoBradesco()
        {
            var banco = Fabricas.BancoFactory.ObterBanco("237", "2");

            var contaBancariaCedente = new ContaBancaria("2374", "4", "0165199", "4");

            var cedente = new Cedente("9999999", 0, "99.999.999/9999-99", "Razão Social X", contaBancariaCedente, null);

            var sacado = new Sacado("Sacado Fulano de Tal", "999.999.999-99", new Endereco()
            {
                TipoLogradouro = "R",
                Logradouro = "1",
                Bairro = "Bairro X",
                Cidade = "Cidade X",
                SiglaUf = "XX",
                Cep = "12345-000",
                Complemento = "Comp X",
                Numero = "9"
            });

            var boleto = new Boleto(cedente, sacado, banco.GetCarteiraCobrancaPorCodigo("06"));
            boleto.NumeroDocumento = "41636135093";
            boleto.ValorBoleto = Convert.ToDecimal(221.40);
            boleto.SequencialNossoNumero = "41636135093";
            boleto.DataVencimento = new DateTime(2014, 07, 10);
            boleto.Moeda = "9";

            banco.FormataNossoNumero(boleto);
            banco.FormataCodigoBarra(boleto);

            const string valorEsperado = "00398391600000015560078000100000000000001238";
            Assert.AreEqual(valorEsperado.Length, boleto.CodigoBarraBoleto.Length);
            Assert.AreEqual(valorEsperado, boleto.CodigoBarraBoleto);
        }
    }
}