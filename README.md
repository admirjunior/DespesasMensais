Controle de Despesas
Um aplicativo de console em C# para gerenciar despesas e receitas, com suporte a categorias, baixas e consulta de saldo por período.

Funcionalidades
Lançar despesas e receitas.
Cadastrar categorias.
Listar despesas, receitas e lançamentos.
Dar baixa em despesas e receitas (múltiplos itens com entrada separada por vírgulas).
Consultar saldo consolidado e previsões por período.
Interface estilizada com Spectre.Console (tabelas com bordas arredondadas e texto em amarelo).
Persistência de dados em arquivo JSON.
Requisitos
.NET SDK 6.0 ou superior.
Pacote Spectre.Console (instalado via NuGet).
Instalação
Clone o repositório:
bash

Copy
git clone https://github.com/seu-usuario/controle-de-despesas.git
Acesse o diretório do projeto:
bash

Copy
cd controle-de-despesas
Instale a dependência Spectre.Console:
bash

Copy
dotnet add package Spectre.Console
Compile e execute o projeto:
bash

Copy
dotnet run
Uso
No menu principal, escolha uma opção (1 a 10) para gerenciar despesas, receitas ou categorias.
Para dar baixa, insira números de itens separados por vírgulas (ex: 1,3).
Os dados são salvos automaticamente em dados.json após cada operação.
Licença
MIT License
