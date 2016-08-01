# Abstractor Framework

# Abstractor.Cqrs

Framework que fornece base para a aplicação do padrão CQRS (Command and Query Responsibility Segregation) de arquitetura, e alguns padrões táticos comuns do DDD (Domain-Driven Design), como Agregações, Entidades,	Objetos de Valor, Eventos de Domínio e de Aplicação.

Fornece um algorítimo genérico de compensação que permite que sejam criadas implementações de Unidades de Trabalho de forma simplificada através de pontos de extensão (ver o módulo Abstractor.Cqrs.AzureStorage, para ver um exemplo de extensão para o serviço de Armazenamento em Nuvem do Azure). Utiliza o paradigma de Programação Orientada a Aspectos (Aspect-Oriented Programming), possibilitando a composição da aplicação de forma altamente desacoplada e modularizada, favorecendo a estruturação de uma arquitetura hexagonal.

Fornece a funcionalidade de logging através de todo o framework, possibilitando o rastreamento completo do ciclo de vida da aplicação.

# Abstractor.Cqrs.EntityFramework

Implementação dos padrões de Repositório Genérico e Unidade de Trabalho para o Entity Framework.
		
# Abstractor.Cqrs.AzureStorage

Implementa o padrão Repositório Genérico para as bibliotecas do serviço de Armazenamento em Nuvem do Azure. Utiliza o algorítimo genérico de compensação, implementado no módulo Abstractor.Cqrs, que permite a execução de operações transacionais entre múltiplos contêineres, tabelas e filas.
		
# Abstractor.Cqrs.UnitOfWork

Coordena e sincroniza os contextos definidos nos módulos Abstractor.Cqrs.AzureStorage e Abstractor.Cqrs.EntityFramework, permitindo operações transacionais entre múltiplos contêineres, tabelas, filas do serviço de Armazenamento em Nuvem do Azure e múltiplas tabelas de bancos de dados, através do Entity Framework.
	
# Abstractor.Cqrs.SimpleInjector

Implementa o padrão Adaptador para o contêiner de inversão de controle do Simple Injector. Deve ser utilizado como base para a implementação futura de adaptadores para outros contêineres.

<hr />

<a href="https://github.com/LeonardoLimaDaSilva/Abstractor/blob/master/README.md">Versão em inglês</a>
