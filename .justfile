# use PowerShell instead of sh:
set windows-shell := ["powershell.exe", "-NoLogo", "-Command"]

alias am := add-migration

hello name:
	clear
	echo "This is an example of using .NET 8 & EFCore {{name}}" 


add-migration name:
	clear
	dotnet ef migrations add {{name}} \
		-p src/DDDEF.Infrastructure -s src/DDDEF.API \
		-c ProjectManagementContext -o EFCore/Migrations