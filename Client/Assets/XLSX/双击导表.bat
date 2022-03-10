@echo off
if exist ..\..\Tools\App\ExcelTool\bin\Release\net6.0 (
cd /d ..\..\Tools\App\ExcelTool\bin\Release\net6.0
)
if exist Tools\App\ExcelTool\bin\Release\net6.0 (
cd /d Tools\App\ExcelTool\bin\Release\net6.0
)


:: --ProjectPath 			【策划】【必改】项目根目录地址
:: --ExportModelType 		【策划】1-导csv 2-导csv+代码
:: --UseHotFix    			【程序】0-非热更  1-热更 
:: --UseServer    			【程序】0-只导出客户端配置  1-客户端配置+导出服务器 2-只导出服务器配置
:: --UseSql       			【程序】0-前后端【不】采用数据库方式  1-前后端采用数据库方式
:: --ExtralFileExtention    【程序】FrameworkConfig.ExtralFileExtention 编辑器如有更改需要此处手动同步一下

dotnet Litframework.ExcelTool.dll ^
--ProjectPath="C:/Personal/Unity/iFunTech/BasePackage/Assets" ^
--ExportModelType=2 ^
--UseHotFix=1 ^
--UseServer=0 ^
--UseSql=0 ^
--ExtralFileExtention="json|dat|assetbundle"

pause