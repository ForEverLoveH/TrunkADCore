# TrunkADCore
这是一个基于c# 实现的winform识别软件，主要用于在学生体侧的体前屈，以及实心球，立定跳远的计数系统，数据库方面主要用的是sqlite，实现对学生数据的增删改，同时调用api 实现将成绩上传到服务器，
在上传中主要是调用使用http 协议 接口 实现将得到的成绩信息上传到远程服务器
在测试上主要是使用肤色识别的算法，也正是由于使用的肤色识别算法，因此对于环境的要求相对来说比较严苛（体前屈，立定跳远）
在实时显示图像上，使用的是第三方库，AFcontroll 实现将相机获取到的信息显示在画面上，当然还有一个c#的画图技术，在页面上显示刻度盘计数，然后再测试的时候实时的显示成绩，当然也可以根据短时间内
获取到的图像去实时修改成绩