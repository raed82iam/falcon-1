# Falcon Local Safe Demo

This profile is local-only and preview-only. It grants no trading, broker, deployment, runtime activation, or business authority.

## Start on Windows PowerShell

```powershell
$env:FALCON_OWNER_EMAIL='raed82iam@gmail.com'
npm run demo:safe
```

Open `http://127.0.0.1:4173` and use the same email in the sign-in form. The password field is ignored in this first local-only profile.

Recovery identity is email-only. The demo stores no password, phone number, broker credential, or provider secret. It is not production authentication and must be replaced by a real identity provider with MFA before public deployment.

The health endpoint is available at `http://127.0.0.1:4173/health`.
