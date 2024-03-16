# SunupIoT 

#### 介绍
SunupIoT 是一款旨在解决各种IoT设备接入，数据收集，数据过滤，数据流转的管理平台(PAAS)，对应用软件提供抽象的业务数据模型，减少应用软件对设备的依赖。

#### 特点

##### IoT设备网关：
目前支持基于MQTT协议的设备连接。内置设备模拟器模拟各种类型的数据，方便在没有真实设备的情况下开发业务模型。
##### 设备安全：
以黑白名单形式建立设备接入规则，对设备进行标识, 对接入的用户名和密码进行效验。
##### 虚拟设备：
以设备虚拟字段的形式存储物理设备数据（内存中），也可以加入虚拟字段存储其他信息，方便数据流转。
##### 数据流转：
基于脚本的形式对数据进行业务处理，在数据发送前和获取后，均可以加入代码做数据调整，满足广泛的业务需求。
##### 应用模型：
建立满足业务的抽象数据结构，屏蔽物理设备，对不同协议可以建立不同的虚拟设备与之对应（目前支持MQTT的server和client）。
##### 数据存储：
满足应用软件对历史数据的需求，存储应用模型数据，目前可以存储在InfluxDB中。
##### 数据脚本引擎：
在周期中支持脚本运行，满足复杂的业务需求，在业务模型和虚拟设备上都可以用脚本对数据进行调整。
##### 可视化管理：
支持Web的平台配置管理，可以同时运行多个项目，每个项目对应不同的业务模型和设备模型。


#### 软件架构
![输入图片说明](https://raw.githubusercontent.com/moto100/SunupIoT/master/READMEPICS/Architecture.png "屏幕截图")

![输入图片说明](https://raw.githubusercontent.com/moto100/SunupIoT/master/READMEPICS/Workflow.png "屏幕截图")



#### 使用说明

1.  下载master分支代码，用VS2022打开Sunup.sln,编译整个解决方案。
2.  如需查看日志请参考《如何查看系统运行日志》配置数据库。
3.  在VS2022中运行"ControlPanelWeb"项目，它会打开默认网页http://localhost:7001，默认用户名和密码是 sunup temptemp。
4. 由于系统有个默认发布的应用，它会运行起来，记住它的网站地址。
5.  在浏览器中打开应用的网站地址，如http://localhost:9001/，会看到《WebSocket Test Page》的默认页面，到此默认的例子程序就运行起来了。
6.  《WebSocket Test Page》的页面，点击《connect》链接WebSocket， 再点击《Subcribe Tags》和《Subcribe Tags(Append to existing subscription)》去订阅数据变化和追加订阅。因为这些Tag绑定到一个内置的模拟器，这时会看到页面不停地有数据返回。此时可以直接再基于这个WebSocket的页面做应用系统的数据对接。

#### 如何让前端单独连接后端平台
1.  Sunup的前端是基于ng-alain中台实现的，这就需要从 https://github.com/moto100/ng-alain/tree/Sunup.ControlPanel （https://gitee.com/moto100/ng-alain/tree/Sunup.ControlPanel/） 下载前端代码，分支名是Sunup.ControlPanel。
2.  在VSCode中打开前端代码Sunup.ControlPanel，编译Angular代码， 最后运行npm run start启动前端站点，此时会打开一个http://localhost:4200，它会默认直接链接到后端http://localhost:7001上获取数据。也可以编译发布版本，成功后把生成的文件拷贝到ControlPanelWeb的网站http://localhost:7001下的wwwroot文件夹下直接使用同一个站点。
3.  在Sunup的前端页面上登入进去，默认用户名和密码是 sunup temptemp
4.  这是也有一个demo APP在运行，其他步骤如上面的《使用说明》的4-6。

#### 如何建一个新的APP
1.  进入站点后点击《新建空应用》按钮新建一个空应用，不做任何改变，保存，再在该应用按钮中选择《导入应用》，导入[Sunup.sln所在文件夹下]SunupDemoApp\PlatformModel.json。再在该应用的下拉按钮中选择《发布运行》，等一会后该应用名前面会出现【已发布】和一个蓝色小点，它代表应用发布并运行起来了。
#### 如何查看系统运行日志
1.  系统使用log4net记录日志，默认使用MSSQL 数据库存储记录， 这时需要修改两个配置文件中的数据连接字符串--IOServerHost\IOServerHost_log4net.config 和 ControlPanelWeb\ControlPanelWeb_appsettings.config，并在数据库里运行DBScript\CreateDB.sql 生成日志表。
#### 如何使用模拟数据测试
1.  系统里有个模拟器设备，可以给应用提供模拟数据，并且数据会有规则变，字段类型 int, 名字为 “int{数字}”， 如 int1，int2,int3...。 这些int数据会随机变化,  字段类型 stepint, 名字为 “stepint{数字}”， 如 stepint1,stepint2...。 这些stepint数据会按步长递增到配置的最大数，之后下降到配置的最小数。字段类型 string, 名字为 “string{数字}”， 如 string1，string2,string3...。 这些string数据会随机变化。字段类型 bool, 名字为 “bool{数字}”， 如 bool1，bool2,bool3...。 这些bool数据会true/false 交替变化。

#### 说明
1.  IOGateway项目是一个发布后可以运行在树莓派4b（Ubantu系统）上的ModbusRTU转MQTT程序.
2.  LicenseMaker项目是一个license制作工具，Sunup在运行时限制业务模型中的节点个数以此达到许可证效果，这个工具用来制作对应节点数的license。运行时Sunup会自动生成bin文件，再在LicenseMaker工具上读入bin文件可以生成出对应的license文件。
3.  SunupIoT除了在链接MQTT模拟器上测试过外，还在连接实际的硬件MQTT PLC设备上测试过，如：zlan 5144J(ModbusToMQTT), 9743(ModbusToMQTT RF433)