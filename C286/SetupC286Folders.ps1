$version = dotnet --version | Out-String
Write-Output $version
if ($version.Substring(0,1) -ne '5')
{
    Write-Output 'Dotnet Core Version 5 is needed.'
    Write-Output 'Please update to the latest Visual Studio.'
    cmd /c 'pause'
	Exit
}
mkdir MyVSCode
cd MyVSCode
dotnet new sln 
for ($x = 1 ; $x -le 13 ; $x++) { 
    $dir = "Lesson{0:d2}" -f $x
    mkdir $dir
    cd $dir
    dotnet new mvc --no-https
    # Remove-Item Controllers\HomeController.cs 
	Set-Content -Path Controllers\HomeController.cs -Value "using System;`r`nusing Microsoft.AspNetCore.Mvc;`r`nnamespace $dir.Controllers`r`n{`r`n  public class HomeController : Controller`r`n  {`r`n    public String Index() => ""$dir Home"";`r`n  }`r`n}`r`n"
	Set-Content -Path Views\Shared\_Layout.cshtml -Value "@RenderBody();"
    Remove-Item Views\_ViewImports.cshtml 
    Remove-Item Views\Shared\_ValidationScriptsPartial.cshtml
    Remove-Item Views\Shared\Error.cshtml
    Remove-Item Views\Home -Recurse -Force -Confirm:$false
	Set-Content -Path wwwroot\index.html -Value "$dir index.html"
    Remove-Item wwwroot\favicon.ico
    Remove-Item wwwroot\css -Recurse -Force -Confirm:$false
    Remove-Item wwwroot\js -Recurse -Force -Confirm:$false
    Remove-Item wwwroot\lib -Recurse -Force -Confirm:$false
    cd ..
    dotnet sln add $dir
}
cd ..
mkdir Databases
mkdir Leo2Lessons
cd Leo2Lessons
for ($x = 1 ; $x -le 13 ; $x++) { 
    $dir = "Week{0:d2}" -f $x
    mkdir $dir
}
cd ..
