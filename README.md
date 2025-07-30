# PureDelivery.IdentityService
🔐 Identity and Authentication Service for Pure Delivery ecosystem - a modern food delivery application.

[31.07.2025]
- Fixed some flows to make it work properly, like AddAddress flow, UpdatePassword flow etc.
- Added logout feature .
- Separated change password and forgot password flows.
- Made refactoring, separated some logic in own classes, separated some endpoints from CustomerController into AuthController

- Nuget changes
   - PureDelivery.Shared.Contracts to v1.9.0
- Fixed response models change in recent Shared nuget project version
- Removed unnecessary flows
- Changed controller routes added name of microservice to make dev process more clear

[27.07.2025]
- Avatar Upload Feature
  - Added AvatarUrl field to CustomerProfile model with migration
  - Integrated Cloudinary service for image storage and processing
  - Implemented CloudinaryService with upload, delete, and URL generation
  - Added CloudinarySettings configuration with transformation options
  - Created avatar upload endpoint with file validation (JPEG, PNG, WebP, 5MB limit)
  - Added automatic image transformations (resize, crop, quality optimization)
- Repository Enhancements
  - Added UpdateAvatarAsync method to ICustomerProfileRepository
  - Added GetByEmailAsync method to ICustomerRepository for inactive customers
  - Updated email confirmation flow to handle non-active customer lookup
- Security & Validation
  - Implemented secure file handling with stream processing
  - Added comprehensive file type and size validation
  - Enhanced error handling and logging for upload operations

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