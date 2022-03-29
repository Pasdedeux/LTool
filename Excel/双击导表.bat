@echo off
if exist ..\Tools\App\ExcelTool\bin\Release\net6.0 (
cd /d ..\Tools\App\ExcelTool\bin\Release\net6.0
)
for %%d in (%~dp0..) do set ParentDirectory=%%~fd%\

::【*】代表需要注意同步编辑器内 FrameworkConfig 对应配置
:: --ExportModelType 		     1-导csv 2-导csv+代码
:: --UseHotFix    			【*】0-非热更  1-热更
:: --UseServer    			     0-只导出客户端配置  1-客户端配置+导出服务器 2-只导出服务器配置
:: --UseSql       			【*】0-用 [CSV] 存配置表  1-用 [SQLite] 存配置表
:: --ExtralFileExtention    【*】FrameworkConfig.ExtralFileExtention

dotnet Litframework.ExcelTool.dll ^
--ExportModelType=2 ^
--UseHotFix=1 ^
--UseServer=1 ^
--UseSql=0 ^
--ExtralFileExtention="json|dat|assetbundle|bin" ^
--ProjectPath=%ParentDirectory%

pause