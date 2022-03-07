@echo off
if exist ..\..\Tools\App\ExcelTool\bin\Release\net6.0 (
cd /d ..\..\Tools\App\ExcelTool\bin\Release\net6.0
)
if exist Tools\App\ExcelTool\bin\Release\net6.0 (
cd /d Tools\App\ExcelTool\bin\Release\net6.0
)

dotnet Litframework.ExcelTool.dll ^
--ProjectPath="C:/Personal/Unity/iFunTech/BasePackage/Assets" ^
--ExportModelType=2 ^
--UseHotFix=1 ^
--UseServer=0 ^
--UseSql=0 ^
--ExtralFileExtention="json|dat|assetbundle"

pause