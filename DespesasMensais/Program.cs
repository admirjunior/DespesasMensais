using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Spectre.Console;

namespace ControleDeDespesas
{
    public class Despesa
    {
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public Categoria Categoria { get; set; }
        public bool Baixa { get; set; }
        public string Descricao { get; set; }
    }

    public class Receita
    {
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public bool Baixa { get; set; }
        public string Descricao { get; set; }
    }

    public class Categoria
    {
        public string Nome { get; set; }
    }

    public class Dados
    {
        public List<Despesa> Despesas { get; set; }
        public List<Receita> Receitas { get; set; }
        public List<Categoria> Categorias { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var despesas = new List<Despesa>();
            var receitas = new List<Receita>();
            var categorias = new List<Categoria>();

            // Carregar dados do JSON
            if (System.IO.File.Exists("dados.json"))
            {
                var json = System.IO.File.ReadAllText("dados.json");
                var dados = JsonSerializer.Deserialize<Dados>(json);
                if (dados != null)
                {
                    despesas = dados.Despesas ?? new List<Despesa>();
                    receitas = dados.Receitas ?? new List<Receita>();
                    categorias = dados.Categorias ?? new List<Categoria>();
                }
            }

            while (true)
            {
                Console.Clear();
                // Cabeçalho estilizado
                AnsiConsole.MarkupLine("[yellow bold]=== Sistema de Controle de Despesas ===[/]");
                AnsiConsole.WriteLine();

                // Tabela de opções do menu
                var menuTable = new Table();
                menuTable.Title = new TableTitle("Menu Principal", new Style(Color.Yellow));
                menuTable.AddColumn(new TableColumn("[yellow]Opção[/]").Centered());
                menuTable.AddColumn(new TableColumn("[yellow]Descrição[/]").Centered());
                menuTable.Border(TableBorder.Rounded);
                menuTable.BorderStyle(new Style(Color.Yellow));

                // Adicionar opções ao menu
                menuTable.AddRow("[yellow]1[/]", "[yellow]Lançar Despesa[/]");
                menuTable.AddRow("[yellow]2[/]", "[yellow]Lançar Receita[/]");
                menuTable.AddRow("[yellow]3[/]", "[yellow]Cadastrar Categoria[/]");
                menuTable.AddRow("[yellow]4[/]", "[yellow]Listar Despesas[/]");
                menuTable.AddRow("[yellow]5[/]", "[yellow]Listar Receitas[/]");
                menuTable.AddRow("[yellow]6[/]", "[yellow]Dar Baixa em Despesa[/]");
                menuTable.AddRow("[yellow]7[/]", "[yellow]Dar Baixa em Receita[/]");
                menuTable.AddRow("[yellow]8[/]", "[yellow]Listar Lançamentos[/]");
                menuTable.AddRow("[yellow]9[/]", "[yellow]Consultar Saldo por Período[/]");
                menuTable.AddRow("[yellow]11[/]", "[yellow]Excluir Lançamentos[/]");
                menuTable.AddRow("[yellow]12[/]", "[yellow]Sair[/]");

                // Renderizar a tabela
                AnsiConsole.Write(menuTable);
                AnsiConsole.WriteLine();
                AnsiConsole.Markup("[yellow]Escolha uma opção: [/]");
                var opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        LancarDespesa(despesas, categorias);
                        break;
                    case "2":
                        LancarReceita(receitas);
                        break;
                    case "3":
                        CadastrarCategoria(categorias);
                        break;
                    case "4":
                        ListarDespesas(despesas);
                        break;
                    case "5":
                        ListarReceitas(receitas);
                        break;
                    case "6":
                        DarBaixaEmDespesa(despesas);
                        break;
                    case "7":
                        DarBaixaEmReceita(receitas);
                        break;
                    case "8":
                        ListarLancamentos(despesas, receitas);
                        break;
                    case "9":
                        ConsultarSaldoPorPeriodo(despesas, receitas);
                        break;
                    case "11":
                        ExcluirLancamentos(despesas, receitas);
                        break;
                    case "12":
                        AnsiConsole.MarkupLine("[yellow]Saindo do sistema...[/]");
                        Thread.Sleep(1000);
                        return;
                    default:
                        AnsiConsole.MarkupLine("[red]Opção inválida. Tente novamente.[/]");
                        Thread.Sleep(1000);
                        break;
                }

                // Salvar dados no JSON
                var dadosJson = new Dados
                {
                    Despesas = despesas,
                    Receitas = receitas,
                    Categorias = categorias
                };
                var json = JsonSerializer.Serialize(dadosJson, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText("dados.json", json);
            }
        }

        static void LancarDespesa(List<Despesa> despesas, List<Categoria> categorias)
        {
            try
            {
                Console.Write("Digite o valor da despesa: R$ ");
                var valor = Convert.ToDecimal(Console.ReadLine());
                Console.Write("Digite a data de vencimento (dd/MM/yyyy): ");
                var dataVencimento = Convert.ToDateTime(Console.ReadLine());
                Console.Write("Digite a descrição: ");
                var descricao = Console.ReadLine();

                Categoria categoriaSelecionada = null;
                if (categorias.Any())
                {
                    // Tabela de Categorias
                    var table = new Table();
                    table.Title = new TableTitle("Categorias Cadastradas", new Style(Color.Yellow));
                    table.AddColumn(new TableColumn("[yellow]Nº[/]").Centered());
                    table.AddColumn(new TableColumn("[yellow]Nome[/]").Centered());
                    table.Border(TableBorder.Rounded);
                    table.BorderStyle(new Style(Color.Yellow));

                    for (int i = 0; i < categorias.Count; i++)
                    {
                        table.AddRow(
                            $"[yellow]{i + 1}[/]",
                            $"[yellow]{categorias[i].Nome}[/]"
                        );
                    }

                    AnsiConsole.WriteLine();
                    AnsiConsole.Write(table);
                    AnsiConsole.WriteLine();

                    AnsiConsole.Markup("[yellow]Digite o número da categoria ou 0 para criar uma nova: [/]");
                    var input = Console.ReadLine();
                    if (int.TryParse(input, out int numero) && numero >= 1 && numero <= categorias.Count)
                    {
                        categoriaSelecionada = categorias[numero - 1];
                    }
                }

                if (categoriaSelecionada == null)
                {
                    AnsiConsole.Markup("[yellow]Digite o nome da nova categoria: [/]");
                    var categoriaNome = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(categoriaNome))
                    {
                        throw new Exception("Nome da categoria não pode ser vazio.");
                    }

                    categoriaSelecionada = categorias.FirstOrDefault(c => c.Nome.Equals(categoriaNome, StringComparison.OrdinalIgnoreCase));
                    if (categoriaSelecionada == null)
                    {
                        categoriaSelecionada = new Categoria { Nome = categoriaNome };
                        categorias.Add(categoriaSelecionada);
                    }
                }

                var despesa = new Despesa
                {
                    Valor = valor,
                    DataVencimento = dataVencimento,
                    Categoria = categoriaSelecionada,
                    Baixa = false,
                    Descricao = descricao
                };
                despesas.Add(despesa);
                AnsiConsole.MarkupLine("[yellow]Despesa lançada com sucesso![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Erro ao lançar despesa: {ex.Message}[/]");
            }
            Thread.Sleep(1000);
        }

        static void LancarReceita(List<Receita> receitas)
        {
            try
            {
                Console.Write("Digite o valor da receita: R$ ");
                var valor = Convert.ToDecimal(Console.ReadLine());
                Console.Write("Digite a data de vencimento (dd/MM/yyyy): ");
                var dataVencimento = Convert.ToDateTime(Console.ReadLine());
                Console.Write("Digite a descrição: ");
                var descricao = Console.ReadLine();

                var receita = new Receita
                {
                    Valor = valor,
                    DataVencimento = dataVencimento,
                    Baixa = false,
                    Descricao = descricao
                };
                receitas.Add(receita);
                AnsiConsole.MarkupLine("[yellow]Receita lançada com sucesso![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Erro ao lançar receita: {ex.Message}[/]");
            }
            Thread.Sleep(1000);
        }

        static void CadastrarCategoria(List<Categoria> categorias)
        {
            Console.Write("Digite o nome da categoria: ");
            var nome = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nome) && !categorias.Any(c => c.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)))
            {
                var categoria = new Categoria { Nome = nome };
                categorias.Add(categoria);
                AnsiConsole.MarkupLine("[yellow]Categoria cadastrada com sucesso![/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Categoria inválida ou já existente.[/]");
            }
            Thread.Sleep(1000);
        }

        static void ListarDespesas(List<Despesa> despesas)
        {
            if (!despesas.Any())
            {
                var table = new Table();
                table.Title = new TableTitle("Despesas", new Style(Color.Yellow));
                table.AddColumn(new TableColumn("[yellow]Mensagem[/]").Centered());
                table.Border(TableBorder.Rounded);
                table.BorderStyle(new Style(Color.Yellow));
                table.AddRow("[yellow]Nenhuma despesa cadastrada.[/]");
                AnsiConsole.WriteLine();
                AnsiConsole.Write(table);
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[yellow]Pressione uma tecla para continuar...[/]");
                Console.ReadKey();
                return;
            }

            // Agrupar despesas por categoria
            var despesasPorCategoria = despesas
                .GroupBy(d => d.Categoria.Nome)
                .OrderBy(g => g.Key);

            foreach (var grupo in despesasPorCategoria)
            {
                var table = new Table();
                table.Title = new TableTitle($"Despesas - Categoria: {grupo.Key}", new Style(Color.Yellow));
                table.AddColumn(new TableColumn("[yellow]Valor[/]").Centered());
                table.AddColumn(new TableColumn("[yellow]Vencimento[/]").Centered());
                table.AddColumn(new TableColumn("[yellow]Descrição[/]").Centered());
                table.AddColumn(new TableColumn("[yellow]Status[/]").Centered());
                table.Border(TableBorder.Rounded);
                table.BorderStyle(new Style(Color.Yellow));

                foreach (var despesa in grupo)
                {
                    var status = despesa.Baixa ? "Baixada" : "Previsão";
                    table.AddRow(
                        $"[yellow]R$ {despesa.Valor:F2}[/]",
                        $"[yellow]{despesa.DataVencimento:dd/MM/yyyy}[/]",
                        $"[yellow]{despesa.Descricao}[/]",
                        $"[yellow]{status}[/]"
                    );
                }

                // Calcular e exibir soma total
                var total = grupo.Sum(d => d.Valor);
                table.AddEmptyRow();
                table.AddRow(
                    "[yellow]Total[/]",
                    "",
                    "",
                    $"[yellow]R$ {total:F2}[/]"
                );

                AnsiConsole.WriteLine();
                AnsiConsole.Write(table);
                AnsiConsole.WriteLine();
            }

            AnsiConsole.MarkupLine("[yellow]Pressione uma tecla para continuar...[/]");
            Console.ReadKey();
        }

        static void ListarReceitas(List<Receita> receitas)
        {
            if (!receitas.Any())
            {
                var table = new Table();
                table.Title = new TableTitle("Receitas", new Style(Color.Yellow));
                table.AddColumn(new TableColumn("[yellow]Mensagem[/]").Centered());
                table.Border(TableBorder.Rounded);
                table.BorderStyle(new Style(Color.Yellow));
                table.AddRow("[yellow]Nenhuma receita cadastrada.[/]");
                AnsiConsole.WriteLine();
                AnsiConsole.Write(table);
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[yellow]Pressione uma tecla para continuar...[/]");
                Console.ReadKey();
                return;
            }

            // Agrupar receitas por descrição
            var receitasPorDescricao = receitas
                .GroupBy(r => r.Descricao)
                .OrderBy(g => g.Key);

            foreach (var grupo in receitasPorDescricao)
            {
                var table = new Table();
                table.Title = new TableTitle($"Receitas - Descrição: {grupo.Key}", new Style(Color.Yellow));
                table.AddColumn(new TableColumn("[yellow]Valor[/]").Centered());
                table.AddColumn(new TableColumn("[yellow]Vencimento[/]").Centered());
                table.AddColumn(new TableColumn("[yellow]Descrição[/]").Centered());
                table.AddColumn(new TableColumn("[yellow]Status[/]").Centered());
                table.Border(TableBorder.Rounded);
                table.BorderStyle(new Style(Color.Yellow));

                foreach (var receita in grupo)
                {
                    var status = receita.Baixa ? "Baixada" : "Previsão";
                    table.AddRow(
                        $"[yellow]R$ {receita.Valor:F2}[/]",
                        $"[yellow]{receita.DataVencimento:dd/MM/yyyy}[/]",
                        $"[yellow]{receita.Descricao}[/]",
                        $"[yellow]{status}[/]"
                    );
                }

                // Calcular e exibir soma total
                var total = grupo.Sum(r => r.Valor);
                table.AddEmptyRow();
                table.AddRow(
                    "[yellow]Total[/]",
                    "",
                    "",
                    $"[yellow]R$ {total:F2}[/]"
                );

                AnsiConsole.WriteLine();
                AnsiConsole.Write(table);
                AnsiConsole.WriteLine();
            }

            AnsiConsole.MarkupLine("[yellow]Pressione uma tecla para continuar...[/]");
            Console.ReadKey();
        }

        static void DarBaixaEmDespesa(List<Despesa> despesas)
        {
            try
            {
                var despesasNaoBaixadas = despesas.Where(d => !d.Baixa).ToList();
                if (!despesasNaoBaixadas.Any())
                {
                    AnsiConsole.MarkupLine("[yellow]Nenhuma despesa pendente para dar baixa.[/]");
                    Thread.Sleep(1000);
                    return;
                }

                // Tabela de Despesas Pendentes
                var table = new Table();
                table.Title = new TableTitle("Despesas Pendentes", new Style(Color.Yellow));
                table.AddColumn(new TableColumn("[yellow]Nº[/]").Centered());
                table.AddColumn(new TableColumn("[yellow]Valor[/]").Centered());
                table.AddColumn(new TableColumn("[yellow]Vencimento[/]").Centered());
                table.AddColumn(new TableColumn("[yellow]Categoria[/]").Centered());
                table.AddColumn(new TableColumn("[yellow]Descrição[/]").Centered());
                table.Border(TableBorder.Rounded);
                table.BorderStyle(new Style(Color.Yellow));

                for (int i = 0; i < despesasNaoBaixadas.Count; i++)
                {
                    var despesa = despesasNaoBaixadas[i];
                    table.AddRow(
                        $"[yellow]{i + 1}[/]",
                        $"[yellow]R$ {despesa.Valor:F2}[/]",
                        $"[yellow]{despesa.DataVencimento:dd/MM/yyyy}[/]",
                        $"[yellow]{despesa.Categoria.Nome}[/]",
                        $"[yellow]{despesa.Descricao}[/]"
                    );
                }

                AnsiConsole.WriteLine();
                AnsiConsole.Write(table);
                AnsiConsole.WriteLine();

                AnsiConsole.Markup("[yellow]Digite os números das despesas para dar baixa (ex: 1,3) ou 0 para cancelar: [/]");
                var input = Console.ReadLine();
                if (input.Trim() == "0")
                {
                    AnsiConsole.MarkupLine("[yellow]Operação cancelada.[/]");
                    Thread.Sleep(1000);
                    return;
                }

                var numeros = input.Split(',')
                    .Select(n => n.Trim())
                    .Where(n => int.TryParse(n, out _))
                    .Select(n => int.Parse(n))
                    .Distinct()
                    .ToList();

                if (!numeros.Any())
                {
                    AnsiConsole.MarkupLine("[red]Nenhum número válido informado.[/]");
                    Thread.Sleep(1000);
                    return;
                }

                var baixasRealizadas = new List<string>();
                var erros = new List<string>();

                foreach (var numero in numeros)
                {
                    if (numero < 1 || numero > despesasNaoBaixadas.Count)
                    {
                        erros.Add($"Número {numero} inválido.");
                        continue;
                    }

                    var despesaSelecionada = despesasNaoBaixadas[numero - 1];
                    if (despesaSelecionada.Baixa)
                    {
                        erros.Add($"Despesa {numero} já está baixada.");
                        continue;
                    }

                    despesaSelecionada.Baixa = true;
                    baixasRealizadas.Add($"Despesa {numero} ({despesaSelecionada.Descricao}) baixada com sucesso.");
                }

                if (baixasRealizadas.Any())
                {
                    foreach (var mensagem in baixasRealizadas)
                    {
                        AnsiConsole.MarkupLine($"[yellow]{mensagem}[/]");
                    }
                }

                if (erros.Any())
                {
                    foreach (var erro in erros)
                    {
                        AnsiConsole.MarkupLine($"[red]{erro}[/]");
                    }
                }

                if (!baixasRealizadas.Any() && erros.Any())
                {
                    AnsiConsole.MarkupLine("[red]Nenhuma baixa foi realizada.[/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Erro ao dar baixa: {ex.Message}[/]");
            }
            Thread.Sleep(1000);
        }

        static void DarBaixaEmReceita(List<Receita> receitas)
        {
            try
            {
                var receitasNaoBaixadas = receitas.Where(r => !r.Baixa).ToList();
                if (!receitasNaoBaixadas.Any())
                {
                    AnsiConsole.MarkupLine("[yellow]Nenhuma receita pendente para dar baixa.[/]");
                    Thread.Sleep(1000);
                    return;
                }

                // Tabela de Receitas Pendentes
                var table = new Table();
                table.Title = new TableTitle("Receitas Pendentes", new Style(Color.Yellow));
                table.AddColumn(new TableColumn("[yellow]Nº[/]").Centered());
                table.AddColumn(new TableColumn("[yellow]Valor[/]").Centered());
                table.AddColumn(new TableColumn("[yellow]Vencimento[/]").Centered());
                table.AddColumn(new TableColumn("[yellow]Descrição[/]").Centered());
                table.Border(TableBorder.Rounded);
                table.BorderStyle(new Style(Color.Yellow));

                for (int i = 0; i < receitasNaoBaixadas.Count; i++)
                {
                    var receita = receitasNaoBaixadas[i];
                    table.AddRow(
                        $"[yellow]{i + 1}[/]",
                        $"[yellow]R$ {receita.Valor:F2}[/]",
                        $"[yellow]{receita.DataVencimento:dd/MM/yyyy}[/]",
                        $"[yellow]{receita.Descricao}[/]"
                    );
                }

                AnsiConsole.WriteLine();
                AnsiConsole.Write(table);
                AnsiConsole.WriteLine();

                AnsiConsole.Markup("[yellow]Digite os números das receitas para dar baixa (ex: 1,3) ou 0 para cancelar: [/]");
                var input = Console.ReadLine();
                if (input.Trim() == "0")
                {
                    AnsiConsole.MarkupLine("[yellow]Operação cancelada.[/]");
                    Thread.Sleep(1000);
                    return;
                }

                var numeros = input.Split(',')
                    .Select(n => n.Trim())
                    .Where(n => int.TryParse(n, out _))
                    .Select(n => int.Parse(n))
                    .Distinct()
                    .ToList();

                if (!numeros.Any())
                {
                    AnsiConsole.MarkupLine("[red]Nenhum número válido informado.[/]");
                    Thread.Sleep(1000);
                    return;
                }

                var baixasRealizadas = new List<string>();
                var erros = new List<string>();

                foreach (var numero in numeros)
                {
                    if (numero < 1 || numero > receitasNaoBaixadas.Count)
                    {
                        erros.Add($"Número {numero} inválido.");
                        continue;
                    }

                    var receitaSelecionada = receitasNaoBaixadas[numero - 1];
                    if (receitaSelecionada.Baixa)
                    {
                        erros.Add($"Receita {numero} já está baixada.");
                        continue;
                    }

                    receitaSelecionada.Baixa = true;
                    baixasRealizadas.Add($"Receita {numero} ({receitaSelecionada.Descricao}) baixada com sucesso.");
                }

                if (baixasRealizadas.Any())
                {
                    foreach (var mensagem in baixasRealizadas)
                    {
                        AnsiConsole.MarkupLine($"[yellow]{mensagem}[/]");
                    }
                }

                if (erros.Any())
                {
                    foreach (var erro in erros)
                    {
                        AnsiConsole.MarkupLine($"[red]{erro}[/]");
                    }
                }

                if (!baixasRealizadas.Any() && erros.Any())
                {
                    AnsiConsole.MarkupLine("[red]Nenhuma baixa foi realizada.[/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Erro ao dar baixa: {ex.Message}[/]");
            }
            Thread.Sleep(1000);
        }

        static void ListarLancamentos(List<Despesa> despesas, List<Receita> receitas)
        {
            // Tabela de Despesas
            var despesaTable = new Table();
            despesaTable.Title = new TableTitle("Despesas", new Style(Color.Yellow));
            despesaTable.AddColumn(new TableColumn("[yellow]Valor[/]").Centered());
            despesaTable.AddColumn(new TableColumn("[yellow]Vencimento[/]").Centered());
            despesaTable.AddColumn(new TableColumn("[yellow]Categoria[/]").Centered());
            despesaTable.AddColumn(new TableColumn("[yellow]Descrição[/]").Centered());
            despesaTable.AddColumn(new TableColumn("[yellow]Status[/]").Centered());
            despesaTable.Border(TableBorder.Rounded);
            despesaTable.BorderStyle(new Style(Color.Yellow));

            if (!despesas.Any())
            {
                despesaTable.AddRow("[yellow]Nenhuma despesa cadastrada.[/]");
            }
            else
            {
                foreach (var despesa in despesas)
                {
                    var status = despesa.Baixa ? "Baixada" : "Previsão";
                    despesaTable.AddRow(
                        $"[yellow]R$ {despesa.Valor:F2}[/]",
                        $"[yellow]{despesa.DataVencimento:dd/MM/yyyy}[/]",
                        $"[yellow]{despesa.Categoria.Nome}[/]",
                        $"[yellow]{despesa.Descricao}[/]",
                        $"[yellow]{status}[/]"
                    );
                }
            }

            // Tabela de Receitas
            var receitaTable = new Table();
            receitaTable.Title = new TableTitle("Receitas", new Style(Color.Yellow));
            receitaTable.AddColumn(new TableColumn("[yellow]Valor[/]").Centered());
            receitaTable.AddColumn(new TableColumn("[yellow]Vencimento[/]").Centered());
            receitaTable.AddColumn(new TableColumn("[yellow]Descrição[/]").Centered());
            receitaTable.AddColumn(new TableColumn("[yellow]Status[/]").Centered());
            receitaTable.Border(TableBorder.Rounded);
            receitaTable.BorderStyle(new Style(Color.Yellow));

            if (!receitas.Any())
            {
                receitaTable.AddRow("[yellow]Nenhuma receita cadastrada.[/]");
            }
            else
            {
                foreach (var receita in receitas)
                {
                    var status = receita.Baixa ? "Baixada" : "Previsão";
                    receitaTable.AddRow(
                        $"[yellow]R$ {receita.Valor:F2}[/]",
                        $"[yellow]{receita.DataVencimento:dd/MM/yyyy}[/]",
                        $"[yellow]{receita.Descricao}[/]",
                        $"[yellow]{status}[/]"
                    );
                }
            }

            // Calcular saldos
            var totalReceitasBaixadas = receitas.Where(r => r.Baixa).Sum(r => r.Valor);
            var totalDespesasBaixadas = despesas.Where(d => d.Baixa).Sum(d => d.Valor);
            var saldoAtual = totalReceitasBaixadas - totalDespesasBaixadas;

            var totalReceitas = receitas.Sum(r => r.Valor);
            var totalDespesas = despesas.Sum(d => d.Valor);
            var saldoPrevisto = totalReceitas - totalDespesas;

            // Tabela de Saldos
            var saldoTable = new Table();
            saldoTable.Title = new TableTitle("Saldos", new Style(Color.Yellow));
            saldoTable.AddColumn(new TableColumn("[yellow]Descrição[/]").Centered());
            saldoTable.AddColumn(new TableColumn("[yellow]Valor[/]").Centered());
            saldoTable.Border(TableBorder.Rounded);
            saldoTable.BorderStyle(new Style(Color.Yellow));

            saldoTable.AddRow("[yellow]Saldo Atual (Baixados)[/]", $"[yellow]R$ {saldoAtual:F2}[/]");
            saldoTable.AddRow("[yellow]Saldo Previsto (Total)[/]", $"[yellow]R$ {saldoPrevisto:F2}[/]");

            // Renderizar as tabelas
            AnsiConsole.WriteLine();
            AnsiConsole.Write(despesaTable);
            AnsiConsole.WriteLine();
            AnsiConsole.Write(receitaTable);
            AnsiConsole.WriteLine();
            AnsiConsole.Write(saldoTable);
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]Pressione uma tecla para continuar...[/]");
            Console.ReadKey();
        }

        static void ConsultarSaldoPorPeriodo(List<Despesa> despesas, List<Receita> receitas)
        {
            try
            {
                Console.Write("Digite o mês (1-12): ");
                var mes = Convert.ToInt32(Console.ReadLine());
                Console.Write("Digite o ano: ");
                var ano = Convert.ToInt32(Console.ReadLine());

                // Considerar apenas lançamentos baixados para o saldo
                var despesasDoMes = despesas.Where(d => d.DataVencimento.Month == mes &&
                                                     d.DataVencimento.Year == ano &&
                                                     d.Baixa).ToList();
                var receitasDoMes = receitas.Where(r => r.DataVencimento.Month == mes &&
                                                     r.DataVencimento.Year == ano &&
                                                     r.Baixa).ToList();

                var totalDespesas = despesasDoMes.Sum(d => d.Valor);
                var totalReceitas = receitasDoMes.Sum(r => r.Valor);
                var saldo = totalReceitas - totalDespesas;

                // Tabela de Saldo Consolidado
                var saldoTable = new Table();
                saldoTable.Title = new TableTitle($"Saldo Consolidado {mes:D2}/{ano}", new Style(Color.Yellow));
                saldoTable.AddColumn(new TableColumn("[yellow]Descrição[/]").Centered());
                saldoTable.AddColumn(new TableColumn("[yellow]Valor[/]").Centered());
                saldoTable.Border(TableBorder.Rounded);
                saldoTable.BorderStyle(new Style(Color.Yellow));

                saldoTable.AddRow("[yellow]Despesas Baixadas[/]", $"[yellow]R$ {totalDespesas:F2}[/]");
                saldoTable.AddRow("[yellow]Receitas Baixadas[/]", $"[yellow]R$ {totalReceitas:F2}[/]");
                saldoTable.AddRow("[yellow]Saldo[/]", $"[yellow]R$ {saldo:F2}[/]");

                // Renderizar tabela de saldo
                AnsiConsole.WriteLine();
                AnsiConsole.Write(saldoTable);
                AnsiConsole.WriteLine();

                Console.Write("Deseja exibir as previsões? (S/N): ");
                var exibirPrevisoes = Console.ReadLine().ToUpper() == "S";

                if (exibirPrevisoes)
                {
                    var despesasPrevistas = despesas.Where(d => d.DataVencimento.Month == mes &&
                                                              d.DataVencimento.Year == ano &&
                                                              !d.Baixa).ToList();
                    var receitasPrevistas = receitas.Where(r => r.DataVencimento.Month == mes &&
                                                             r.DataVencimento.Year == ano &&
                                                             !r.Baixa).ToList();

                    var totalDespesasPrevistas = despesasPrevistas.Sum(d => d.Valor);
                    var totalReceitasPrevistas = receitasPrevistas.Sum(r => r.Valor);
                    var saldoPrevisto = totalReceitasPrevistas - totalDespesasPrevistas;

                    // Tabela de Previsões
                    var previsaoTable = new Table();
                    previsaoTable.Title = new TableTitle($"Previsões {mes:D2}/{ano}", new Style(Color.Yellow));
                    previsaoTable.AddColumn(new TableColumn("[yellow]Descrição[/]").Centered());
                    previsaoTable.AddColumn(new TableColumn("[yellow]Valor[/]").Centered());
                    previsaoTable.Border(TableBorder.Rounded);
                    previsaoTable.BorderStyle(new Style(Color.Yellow));

                    previsaoTable.AddRow("[yellow]Despesas Previstas[/]", $"[yellow]R$ {totalDespesasPrevistas:F2}[/]");
                    previsaoTable.AddRow("[yellow]Receitas Previstas[/]", $"[yellow]R$ {totalReceitasPrevistas:F2}[/]");
                    previsaoTable.AddRow("[yellow]Saldo Previsto[/]", $"[yellow]R$ {saldoPrevisto:F2}[/]");

                    // Renderizar tabela de previsões
                    AnsiConsole.Write(previsaoTable);
                    AnsiConsole.WriteLine();

                    // Tabela de Detalhes de Despesas Previstas
                    if (despesasPrevistas.Any())
                    {
                        var despesaPrevistaTable = new Table();
                        despesaPrevistaTable.Title = new TableTitle("Detalhe das Despesas Previstas", new Style(Color.Yellow));
                        despesaPrevistaTable.AddColumn(new TableColumn("[yellow]Valor[/]").Centered());
                        despesaPrevistaTable.AddColumn(new TableColumn("[yellow]Vencimento[/]").Centered());
                        despesaPrevistaTable.AddColumn(new TableColumn("[yellow]Categoria[/]").Centered());
                        despesaPrevistaTable.AddColumn(new TableColumn("[yellow]Descrição[/]").Centered());
                        despesaPrevistaTable.Border(TableBorder.Rounded);
                        despesaPrevistaTable.BorderStyle(new Style(Color.Yellow));

                        foreach (var d in despesasPrevistas)
                        {
                            despesaPrevistaTable.AddRow(
                                $"[yellow]R$ {d.Valor:F2}[/]",
                                $"[yellow]{d.DataVencimento:dd/MM/yyyy}[/]",
                                $"[yellow]{d.Categoria.Nome}[/]",
                                $"[yellow]{d.Descricao}[/]"
                            );
                        }

                        AnsiConsole.Write(despesaPrevistaTable);
                        AnsiConsole.WriteLine();
                    }

                    // Tabela de Detalhes de Receitas Previstas
                    if (receitasPrevistas.Any())
                    {
                        var receitaPrevistaTable = new Table();
                        receitaPrevistaTable.Title = new TableTitle("Detalhe das Receitas Previstas", new Style(Color.Yellow));
                        receitaPrevistaTable.AddColumn(new TableColumn("[yellow]Valor[/]").Centered());
                        receitaPrevistaTable.AddColumn(new TableColumn("[yellow]Vencimento[/]").Centered());
                        receitaPrevistaTable.AddColumn(new TableColumn("[yellow]Descrição[/]").Centered());
                        receitaPrevistaTable.Border(TableBorder.Rounded);
                        receitaPrevistaTable.BorderStyle(new Style(Color.Yellow));

                        foreach (var r in receitasPrevistas)
                        {
                            receitaPrevistaTable.AddRow(
                                $"[yellow]R$ {r.Valor:F2}[/]",
                                $"[yellow]{r.DataVencimento:dd/MM/yyyy}[/]",
                                $"[yellow]{r.Descricao}[/]"
                            );
                        }

                        AnsiConsole.Write(receitaPrevistaTable);
                        AnsiConsole.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Erro ao consultar saldo: {ex.Message}[/]");
            }
            AnsiConsole.MarkupLine("[yellow]Pressione uma tecla para continuar...[/]");
            Console.ReadKey();
        }

        static void ExcluirLancamentos(List<Despesa> despesas, List<Receita> receitas)
        {
            try
            {
                if (!despesas.Any() && !receitas.Any())
                {
                    AnsiConsole.MarkupLine("[yellow]Nenhum lançamento cadastrado para excluir.[/]");
                    Thread.Sleep(1000);
                    return;
                }

                // Tabela de Despesas
                var despesaTable = new Table();
                despesaTable.Title = new TableTitle("Despesas", new Style(Color.Yellow));
                despesaTable.AddColumn(new TableColumn("[yellow]Nº[/]").Centered());
                despesaTable.AddColumn(new TableColumn("[yellow]Valor[/]").Centered());
                despesaTable.AddColumn(new TableColumn("[yellow]Vencimento[/]").Centered());
                despesaTable.AddColumn(new TableColumn("[yellow]Categoria[/]").Centered());
                despesaTable.AddColumn(new TableColumn("[yellow]Descrição[/]").Centered());
                despesaTable.AddColumn(new TableColumn("[yellow]Status[/]").Centered());
                despesaTable.Border(TableBorder.Rounded);
                despesaTable.BorderStyle(new Style(Color.Yellow));

                if (!despesas.Any())
                {
                    despesaTable.AddRow("[yellow]Nenhuma despesa cadastrada.[/]");
                }
                else
                {
                    for (int i = 0; i < despesas.Count; i++)
                    {
                        var despesa = despesas[i];
                        var status = despesa.Baixa ? "Baixada" : "Previsão";
                        despesaTable.AddRow(
                            $"[yellow]{i + 1}[/]",
                            $"[yellow]R$ {despesa.Valor:F2}[/]",
                            $"[yellow]{despesa.DataVencimento:dd/MM/yyyy}[/]",
                            $"[yellow]{despesa.Categoria.Nome}[/]",
                            $"[yellow]{despesa.Descricao}[/]",
                            $"[yellow]{status}[/]"
                        );
                    }
                }

                // Tabela de Receitas
                var receitaTable = new Table();
                receitaTable.Title = new TableTitle("Receitas", new Style(Color.Yellow));
                receitaTable.AddColumn(new TableColumn("[yellow]Nº[/]").Centered());
                receitaTable.AddColumn(new TableColumn("[yellow]Valor[/]").Centered());
                receitaTable.AddColumn(new TableColumn("[yellow]Vencimento[/]").Centered());
                receitaTable.AddColumn(new TableColumn("[yellow]Descrição[/]").Centered());
                receitaTable.AddColumn(new TableColumn("[yellow]Status[/]").Centered());
                receitaTable.Border(TableBorder.Rounded);
                receitaTable.BorderStyle(new Style(Color.Yellow));

                if (!receitas.Any())
                {
                    receitaTable.AddRow("[yellow]Nenhuma receita cadastrada.[/]");
                }
                else
                {
                    for (int i = 0; i < receitas.Count; i++)
                    {
                        var receita = receitas[i];
                        var status = receita.Baixa ? "Baixada" : "Previsão";
                        receitaTable.AddRow(
                            $"[yellow]{i + 1}[/]",
                            $"[yellow]R$ {receita.Valor:F2}[/]",
                            $"[yellow]{receita.DataVencimento:dd/MM/yyyy}[/]",
                            $"[yellow]{receita.Descricao}[/]",
                            $"[yellow]{status}[/]"
                        );
                    }
                }

                // Renderizar as tabelas
                AnsiConsole.WriteLine();
                AnsiConsole.Write(despesaTable);
                AnsiConsole.WriteLine();
                AnsiConsole.Write(receitaTable);
                AnsiConsole.WriteLine();

                AnsiConsole.Markup("[yellow]Digite os números dos lançamentos para excluir (ex: 1,3 para despesas, R1,R3 para receitas) ou 0 para cancelar: [/]");
                var input = Console.ReadLine();
                if (input.Trim() == "0")
                {
                    AnsiConsole.MarkupLine("[yellow]Operação cancelada.[/]");
                    Thread.Sleep(1000);
                    return;
                }

                var despesasParaExcluir = new List<(int Index, Despesa Despesa)>();
                var receitasParaExcluir = new List<(int Index, Receita Receita)>();
                var erros = new List<string>();

                // Processar entrada
                var entradas = input.Split(',')
                    .Select(n => n.Trim())
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .ToList();

                foreach (var entrada in entradas)
                {
                    if (entrada.StartsWith("R", StringComparison.OrdinalIgnoreCase))
                    {
                        // Receita
                        if (int.TryParse(entrada.Substring(1), out int numero) && numero >= 1 && numero <= receitas.Count)
                        {
                            receitasParaExcluir.Add((numero - 1, receitas[numero - 1]));
                        }
                        else
                        {
                            erros.Add($"Número de receita {entrada} inválido.");
                        }
                    }
                    else
                    {
                        // Despesa
                        if (int.TryParse(entrada, out int numero) && numero >= 1 && numero <= despesas.Count)
                        {
                            despesasParaExcluir.Add((numero - 1, despesas[numero - 1]));
                        }
                        else
                        {
                            erros.Add($"Número de despesa {entrada} inválido.");
                        }
                    }
                }

                if (!despesasParaExcluir.Any() && !receitasParaExcluir.Any() && erros.Any())
                {
                    foreach (var erro in erros)
                    {
                        AnsiConsole.MarkupLine($"[red]{erro}[/]");
                    }
                    AnsiConsole.MarkupLine("[red]Nenhum lançamento válido informado.[/]");
                    Thread.Sleep(1000);
                    return;
                }

                // Confirmar exclusão
                var exclusoesRealizadas = new List<string>();

                foreach (var (index, despesa) in despesasParaExcluir.OrderByDescending(d => d.Index))
                {
                    AnsiConsole.Markup($"[yellow]Tem certeza que deseja excluir a despesa '{despesa.Descricao}' (R$ {despesa.Valor:F2})? (S/N): [/]");
                    var confirmacao = Console.ReadLine().ToUpper();
                    if (confirmacao == "S")
                    {
                        despesas.RemoveAt(index);
                        exclusoesRealizadas.Add($"Despesa {index + 1} ({despesa.Descricao}) excluída com sucesso.");
                    }
                    else
                    {
                        erros.Add($"Exclusão da despesa {index + 1} ({despesa.Descricao}) cancelada.");
                    }
                }

                foreach (var (index, receita) in receitasParaExcluir.OrderByDescending(r => r.Index))
                {
                    AnsiConsole.Markup($"[yellow]Tem certeza que deseja excluir a receita '{receita.Descricao}' (R$ {receita.Valor:F2})? (S/N): [/]");
                    var confirmacao = Console.ReadLine().ToUpper();
                    if (confirmacao == "S")
                    {
                        receitas.RemoveAt(index);
                        exclusoesRealizadas.Add($"Receita {index + 1} ({receita.Descricao}) excluída com sucesso.");
                    }
                    else
                    {
                        erros.Add($"Exclusão da receita {index + 1} ({receita.Descricao}) cancelada.");
                    }
                }

                // Exibir resultados
                if (exclusoesRealizadas.Any())
                {
                    foreach (var mensagem in exclusoesRealizadas)
                    {
                        AnsiConsole.MarkupLine($"[yellow]{mensagem}[/]");
                    }
                }

                if (erros.Any())
                {
                    foreach (var erro in erros)
                    {
                        AnsiConsole.MarkupLine($"[red]{erro}[/]");
                    }
                }

                if (!exclusoesRealizadas.Any() && erros.Any())
                {
                    AnsiConsole.MarkupLine("[red]Nenhuma exclusão foi realizada.[/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Erro ao excluir lançamentos: {ex.Message}[/]");
            }
            Thread.Sleep(1000);
        }
    }
}