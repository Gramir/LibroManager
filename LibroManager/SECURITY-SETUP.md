# LibroManager Security Setup Guide

## Important Security Configuration

This application has been configured with enhanced security settings. Please follow these steps for proper setup:

### 1. Email Configuration

The application requires SMTP settings to be configured. For security reasons, these credentials are not stored in the source code.

#### Option A: Using User Secrets (Recommended for Development)
```bash
dotnet user-secrets set "EmailSettings:SmtpServer" "your-smtp-server.com"
dotnet user-secrets set "EmailSettings:SmtpPort" "587"
dotnet user-secrets set "EmailSettings:SmtpUsername" "your-username"
dotnet user-secrets set "EmailSettings:SmtpPassword" "your-password"
```

#### Option B: Using Environment Variables (Recommended for Production)
```bash
export EmailSettings__SmtpServer="your-smtp-server.com"
export EmailSettings__SmtpPort="587"
export EmailSettings__SmtpUsername="your-username"
export EmailSettings__SmtpPassword="your-password"
```

### 2. Default Admin User

On first run, the application will create a default admin user:
- **Email**: admin@libromanager.com
- **Password**: A secure random password will be generated and logged to the console

**IMPORTANT**: Change this password immediately after first login!

### 3. Password Requirements

The application enforces strong password policies:
- Minimum 8 characters
- Must contain uppercase letter
- Must contain lowercase letter
- Must contain digit
- Must contain special character

### 4. Security Features Implemented

- **Strong Password Policy**: Enhanced password requirements
- **Secure Cookies**: HttpOnly, Secure, SameSite protection
- **Session Management**: 8-hour session timeout with sliding expiration
- **Open Redirect Prevention**: Enhanced URL validation
- **Security Headers**: X-Content-Type-Options, X-Frame-Options, CSP, etc.
- **HTTPS Enforcement**: HSTS enabled in production
- **Account Lockout**: Protection against brute force attacks

### 5. Production Deployment Checklist

- [ ] Configure SMTP settings using environment variables
- [ ] Enable HTTPS/TLS
- [ ] Change default admin password
- [ ] Review and customize Content Security Policy
- [ ] Set up proper logging and monitoring
- [ ] Configure backup strategy for SQLite database
- [ ] Review user roles and permissions
- [ ] Test all authentication flows

### 6. Security Best Practices

1. **Regular Updates**: Keep all NuGet packages up to date
2. **Access Control**: Regularly review user permissions
3. **Monitoring**: Monitor failed login attempts
4. **Backup**: Regularly backup the database
5. **HTTPS**: Always use HTTPS in production
6. **Environment Separation**: Use different configurations for dev/staging/prod

### 7. Reporting Security Issues

If you discover any security vulnerabilities, please report them responsibly by contacting the maintainers privately.