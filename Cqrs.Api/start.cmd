SET ASPNETCORE_ENVIRONMENT=Development
SET LAUNCHER_PATH=bin\Debug\net7.0\Cqrs.Api.exe
"C:\Program Files\IIS Express\iisexpress.exe" /config:"..\.vs\Cqrs.Api\config\applicationhost.config" /site:"Cqrs.Api" /apppool:"Cqrs.Api AppPool"
