import { AuthProvider, AuthResult, WebSurfaceGrant } from './auth.js';

const normalizeEmail=value=>String(value??'').trim().toLowerCase();

export function validateLocalSafeOwnerConfig(config){
  if (!config || config.environment !== 'LOCAL_SAFE_DEMO') throw new TypeError('local safe Owner environment is invalid');
  const email=normalizeEmail(config.ownerEmail);
  if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) throw new TypeError('a valid Owner recovery email is required');
  if (config.businessAuthorityGranted !== false || config.tradingAuthorityGranted !== false || config.brokerAuthorityGranted !== false) {
    throw new TypeError('local safe Owner cannot receive business, trading, or broker authority');
  }
  return Object.freeze({...config,ownerEmail:email});
}

export function createLocalSafeOwnerAuthAdapter(input){
  const config=validateLocalSafeOwnerConfig(input);
  const rejected=()=>Object.freeze({state:AuthResult.REJECTED,provider:AuthProvider.PASSWORD,role:null,applications:[],challenge:null,authoritativeSession:false,principalId:null,surfaceGrants:[],businessAuthorityGranted:false});
  return Object.freeze({
    async signIn(credentials={}){
      if (normalizeEmail(credentials.email ?? credentials.username)!==config.ownerEmail) return rejected();
      return Object.freeze({
        state:AuthResult.AUTHENTICATED,
        provider:AuthProvider.PASSWORD,
        role:'PROJECT_OWNER',
        applications:['FSATS'],
        challenge:null,
        authoritativeSession:true,
        principalId:'LOCAL-DEMO-OWNER',
        sessionId:`LOCAL-DEMO-${Date.now()}`,
        surfaceGrants:Object.freeze([WebSurfaceGrant.OWNER]),
        businessAuthorityGranted:false,
        tradingAuthorityGranted:false,
        brokerAuthorityGranted:false,
        recoveryMethod:'EMAIL_ONLY',
        recoveryEmail:config.ownerEmail,
        localSafeDemo:true
      });
    },
    async signInWithProvider(){ return rejected(); },
    async verifyMfa(){ return rejected(); }
  });
}
