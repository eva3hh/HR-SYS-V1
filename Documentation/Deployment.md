# نشر التطبيق (Deployment Guide)

## المتطلبات قبل النشر

### الخادم
- Windows Server 2019+ أو Linux
- .NET 9 Runtime
- SQL Server 2019+
- Node.js 18+ (للـ Frontend)

### التحضير

1. **نسخ المشروع**
```bash
git clone https://github.com/eva3hh/HR-SYS-V1.git
cd HR-SYS-V1
```

2. **إعداد متغيرات البيئة**
```bash
# Backend
cd Backend/SmartHRAPI
cp appsettings.example.json appsettings.Production.json
# عدّل appsettings.Production.json بإضافة بيانات الإنتاج

# Frontend
cd ../../Frontend/smart-hr-app
cp .env.example .env.production
```

## النشر باستخدام Docker

### البناء والتشغيل

```bash
# بناء الصور
docker-compose build

# تشغيل الحاويات
docker-compose up -d

# التحقق من الحالة
docker-compose ps

# مشاهدة السجلات
docker-compose logs -f api
docker-compose logs -f frontend
```

### إيقاف التطبيق

```bash
docker-compose down
```

## النشر على Windows Server

### 1. إعداد IIS

```powershell
# تثبيت دور IIS
Install-WindowsFeature Web-Server -IncludeManagementTools

# تثبيت .NET Hosting Bundle
# انزّل من https://dotnet.microsoft.com/download/dotnet
```

### 2. نشر الـ Backend

```bash
# نشر التطبيق
dotnet publish -c Release -o .\publish

# نسخ الملفات إلى IIS
Copy-Item -Path ".\publish\*" -Destination "C:\inetpub\wwwroot\smarthr-api" -Recurse
```

### 3. نشر الـ Frontend

```bash
cd Frontend/smart-hr-app
npm run build

# نسخ ملفات dist إلى IIS
Copy-Item -Path ".\dist\*" -Destination "C:\inetpub\wwwroot\smarthr" -Recurse
```

## النشر على Linux

### 1. تثبيت المتطلبات

```bash
# تثبيت .NET
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh

# تثبيت Node.js
curl -sL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt-get install -y nodejs

# تثبيت SQL Server
curl https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
sudo add-apt-repository "$(wget -qO- https://packages.microsoft.com/config/ubuntu/20.04/mssql-server-2022.list)"
sudo apt-get install -y mssql-server
```

### 2. إنشاء خدمة Systemd

```bash
# نشر الـ Backend
cd /opt/smarthr
sudo dotnet publish -c Release -o ./publish

# إنشاء ملف الخدمة
sudo nano /etc/systemd/system/smarthr-api.service
```

```ini
[Unit]
Description=Smart HR API
After=network.target

[Service]
Type=notify
User=smarthr
WorkingDirectory=/opt/smarthr/publish
ExecStart=/usr/bin/dotnet /opt/smarthr/publish/SmartHRAPI.dll
Restart=always
RestartSec=10

[Install]
WantedBy=multi-user.target
```

### 3. تشغيل الخدمة

```bash
sudo systemctl daemon-reload
sudo systemctl enable smarthr-api
sudo systemctl start smarthr-api
sudo systemctl status smarthr-api
```

## النسخ الاحتياطي والاسترجاع

### نسخ احتياطي يومي من قاعدة البيانات

```bash
# Windows
# إنشاء مهمة مجدولة في Task Scheduler

# Linux
# أضف إلى crontab
0 2 * * * /usr/bin/sqlcmd -S localhost -U sa -P password -Q "BACKUP DATABASE SmartHRDB TO DISK='/backup/smarthr_$(date +%Y%m%d).bak'"
```

### استرجاع النسخة الاحتياطية

```sql
RESTORE DATABASE SmartHRDB
FROM DISK = '/backup/smarthr_20240626.bak'
WITH REPLACE;
```

## المراقبة والصيانة

### تفعيل السجلات

```bash
# في appsettings.Production.json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft": "Warning"
  },
  "File": {
    "Path": "/var/log/smarthr/api.log"
  }
}
```

### مراقبة الأداء

```bash
# استخدام Application Insights
# أو ELK Stack
```

## الأمان

1. **تحديث كلمات المرور الافتراضية**
2. **تفعيل HTTPS**
3. **إعداد جدار الحماية (Firewall)**
4. **تحديثات الأمان المنتظمة**
5. **نسخ احتياطية منتظمة**
6. **مراقبة السجلات**

## استكشاف الأخطاء

### مشكلة: الاتصال برفع قاعدة البيانات

```bash
# تحقق من اتصال SQL Server
sqlcmd -S localhost -U sa -P password -Q "SELECT @@version"

# تحقق من سلسلة الاتصال
# تأكد من أن اسم الخادم والمستخدم والكلمة الحالية صحيحة
```

### مشكلة: الـ Frontend لا يتصل بـ API

```bash
# تحقق من CORS
# تأكد من أن العنوان الأمامي مدرج في CORS

# اختبر الاتصال
curl http://localhost:5000/api/health
```

### مشكلة: استهلاك الذاكرة العالي

```bash
# تحقق من حجم السجلات
ls -lh /var/log/smarthr/

# امسح السجلات القديمة
find /var/log/smarthr/ -mtime +30 -delete
```

## التطوير والاختبار المستقبلي

- إضافة اختبارات الوحدة (Unit Tests)
- اختبارات التكامل (Integration Tests)
- اختبارات الحمل (Load Testing)
- أتمتة النشر (CI/CD)
