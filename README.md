# Smart HR ERP

## نظام إدارة الموارد البشرية الاحترافي

### المميزات الرئيسية

✅ **لوحة تحكم احترافية** - رسوم بيانية وإحصائيات فورية
✅ **إدارة الموظفين** - بيانات شاملة ووثائق رقمية
✅ **نظام الحضور والانصراف** - دعم أجهزة البصمة المتعددة
✅ **إدارة الإجازات** - أنواع متعددة من الإجازات والأذونات
✅ **حساب الرواتب التلقائي** - حسابات دقيقة ومرنة
✅ **تقارير متقدمة** - تصدير PDF و Excel
✅ **نظام صلاحيات دقيق** - تحكم كامل بالوصول
✅ **واجهة عربية** - دعم كامل للعربية والإنجليزية
✅ **وضع ليلي** - واجهة مريحة للعين
✅ **متوافق مع الأجهزة** - Responsive Design

## المتطلبات

- .NET 9.0 أو أحدث
- SQL Server 2019 أو أحدث
- Node.js 18 أو أحدث
- Docker (اختياري)

## البدء السريع

### التثبيت باستخدام Docker

```bash
docker-compose up -d
```

ثم اذهب إلى: http://localhost:3000

### التثبيت اليدوي

#### Backend
```bash
cd Backend/SmartHRAPI
dotnet restore
dotnet ef database update
dotnet run
```

#### Frontend
```bash
cd Frontend/smart-hr-app
npm install
npm run dev
```

## الوثائق

- [API Documentation](Documentation/API.md)
- [Architecture](Documentation/Architecture.md)
- [Deployment Guide](Documentation/Deployment.md)
- [Contributing](CONTRIBUTING.md)

## التكنولوجيا المستخدمة

### Backend
- ASP.NET Core 9
- Entity Framework Core
- SQL Server
- JWT Authentication

### Frontend
- React 18
- TypeScript
- Material UI
- Recharts

## الترخيص

MIT License - انظر LICENSE.md للمزيد

## التواصل والدعم

للمزيد من المعلومات:
- البريد الإلكتروني: support@smarthr.com
- الموقع الرسمي: https://smarthr.com

## المساهمون

شكر خاص لجميع المساهمين في هذا المشروع.

---

**تم التطوير بواسطة فريق Smart HR**
