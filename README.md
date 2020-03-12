# MAAV - Modelo Automatizado de Atualização de Versão

Atualmente, as aplicações estão mais complexas e envolvem equipes multidisciplinares. Nesse sentido, o processo de desenvolvimento de software também precisou se adaptar para atender as demandas dos usuários. Uma parte muito importante nesse processo é o controle de versão de software, também chamado de versionamento semântico. O objetivo desse trabalho é implementar um programa que provê uma forma automatizada para controlar números de versões para aplicações.

## Getting Started

Clone o repositorio no diretório do projeto e com o .Net Core 3.1 instalado execute o comando 

```
 ./run.sh
```

A aplicação utiliza o LiteDB localmente mas esta pode ser modificada para o banco de dados MongoDB.
Desta forma será executado o build do projeto e abrirá uma porta conforme o arquivo src/maav.webapi/Properties/launchSettings.json estiver configurado(por padrão: 5892)

De forma mais simples temos a execução da aplicação via docker build e orquestão via docker-compose, onde podemos executar o seguinte arquivo:

```
./run.sh docker
```

### Prerequisites
- .Net Core 3.1
- Docker
- MongoDB

## Versioning

Usamos  [SemVer](http://semver.org/) para versionamento. Para as versões disponíveis , [temos a lista de tags](https://github.com/AugustoDeveloper/maav/tags). 


## Authors

* **Augusto Mesquita** - [Perfil](https://github.com/AugustoDeveloper)