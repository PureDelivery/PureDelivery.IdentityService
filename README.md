# PureDelivery.IdentityService
🔐 Identity and Authentication Service for Pure Delivery ecosystem - a modern food delivery application.

[26.07.2025]
- Whole workflow of controllers
- Nuget upgrade
	- PureDelivery.Shared.Contracts to 1.8.0
- Removed logic of PrefPaymentMethod field manipulation during registration
- Email confirmation flow: 
	- Otp config
	- Customer core model fix
	- Migration created
	- Mappers adjustments
	- Added emailConfig and otpSettings config classes and jsons
	- Customer service adjustments
		- Created OtpService and EmailService to create an otp code and send it to the email

[15.07.2025]
- Implemented automatic client info extraction in authenticate endpoint
- Added GetUserAgent() and GetClientIpAddress() methods with fallback priorities
- Enhanced security by preventing client-side manipulation of IP/UserAgent
- Added Gateway integration support with X-Gateway-* headers
- Implemented session cookie setting with security options
- Created initial project structure (API, Core, Infrastructure)
- Created infrastructure project with DbContext
- Created repository interfaces and implementations
- Implemented Customer service with full CRUD operations
- Added Entity Framework configurations for all models
- Created initial database migrations
- Integrated custom NuGet packages for configuration and Redis
- Added Swagger documentation with security definitions
- Configured Serilog for structured logging

[12.07.2025]
- Created initial project structure
- Created infrastructure project with db context
- Created repository interfaces 
- Implemented repository interfaces in infra project