import{a as Ke,b as Xe}from"./chunk-2E7LSROC.js";import{a as ke,h as Te,l as Ye}from"./chunk-UT5DQQKT.js";import{a as pe,b as V,c as se}from"./chunk-B4V3RE4H.js";import{Aa as X,Ba as J,Da as Q,Ga as Ae,Ha as Fe,I as Pe,Ia as Oe,Ja as Le,K as De,Ka as ce,La as de,Ma as qe,Na as Ge,O as Re,Oa as $e,Pa as We,Qa as je,Ra as He,e as Ie,g as U,h as f,ha as Be,i as A,ia as Ve,j as F,ja as Ue,k as Me,m as Ee,ma as $,o as O,oa as W,q as L,ra as j,t as q,wa as H,x as le,xa as Y,ya as me,z as G,za as K}from"./chunk-EA4M7BGS.js";import{$a as te,Af as xe,Bf as x,Cb as s,Db as l,Eb as we,Fa as z,Gb as w,Ha as a,Hf as k,I as ee,Ic as R,Jc as oe,Jf as I,Lf as T,Lg as B,Ma as ve,Ob as re,Og as M,Qa as C,Rg as ae,Va as o,Wa as p,Xa as g,Y as c,_ as S,cb as b,eb as E,fa as P,ga as D,hb as Ce,lc as ne,nb as N,ob as m,pb as v,qb as h,sb as _e,tb as Se,ub as be,vb as ie,wb as y,xa as r,xb as ye,yb as Ne}from"./chunk-MUNQRYA4.js";import{a as fe}from"./chunk-GAL4ENT6.js";var Je=(()=>{class e{constructor(){this.socialService=c(V),this.settingsSrv=c(B),this.type=""}ngOnInit(){this.mockModel()}mockModel(){let t={token:"123456789",name:"cipchk",email:`${this.type}@${this.type}.com`,id:1e4,time:+new Date};this.settingsSrv.setUser(fe(fe({},this.settingsSrv.user),t)),this.socialService.callback(t)}static{this.\u0275fac=function(i){return new(i||e)}}static{this.\u0275cmp=S({type:e,selectors:[["app-callback"]],inputs:{type:"type"},standalone:!0,features:[ie([V]),y],decls:0,vars:0,template:function(i,n){},encapsulation:2})}}return e})();var it=(()=>{class e{constructor(){this.tokenService=c(pe),this.settings=c(B),this.router=c(R),this.f=new Me({password:new Ee("",{nonNullable:!0,validators:[f.required]})})}get user(){return this.settings.user}submit(){this.f.controls.password.markAsDirty(),this.f.controls.password.updateValueAndValidity(),this.f.valid&&(console.log("Valid!"),console.log(this.f.value),this.tokenService.set({token:"123"}),this.router.navigate(["dashboard"]))}static{this.\u0275fac=function(i){return new(i||e)}}static{this.\u0275cmp=S({type:e,selectors:[["passport-lock"]],standalone:!0,features:[y],decls:15,vars:11,consts:[[1,"ant-card","width-lg",2,"margin","0 auto"],[1,"ant-card-body"],[1,"avatar"],["nzIcon","user","nzSize","large",3,"nzSrc"],["nz-form","","role","form",1,"mt-md",3,"ngSubmit","formGroup"],[3,"nzErrorTip"],["nzSuffixIcon","lock"],["type","password","nz-input","","formControlName","password"],["nz-row","","nzType","flex","nzAlign","middle"],["nz-col","",2,"text-align","right",3,"nzOffset","nzSpan"],["nz-button","","nzType","primary",3,"disabled"]],template:function(i,n){i&1&&(o(0,"div",0)(1,"div",1)(2,"div",2),g(3,"nz-avatar",3),p(),o(4,"form",4),b("ngSubmit",function(){return n.submit()}),o(5,"nz-form-item")(6,"nz-form-control",5),s(7,"i18n"),o(8,"nz-input-group",6),g(9,"input",7),p()()(),o(10,"div",8)(11,"div",9)(12,"button",10),m(13),s(14,"i18n"),p()()()()()()),i&2&&(r(3),a("nzSrc",n.user.avatar),r(),a("formGroup",n.f),r(2),a("nzErrorTip",l(7,7,"validation.password.required")),r(5),a("nzOffset",12)("nzSpan",12),r(),a("disabled",!n.f.valid),r(),v(l(14,9,"app.lock")))},dependencies:[G,O,U,A,F,L,q,M,Ge,qe,Q,Y,H,X,K,J,me,T,I,x,k,j,$,W],styles:["[_nghost-%COMP%]     .ant-card-body{position:relative;margin-top:80px}[_nghost-%COMP%]     .avatar{position:absolute;top:-20px;left:50%;margin-left:-20px}"]})}}return e})();function mt(e,d){if(e&1&&g(0,"nz-alert",3),e&2){let t=E();a("nzType","error")("nzMessage",t.error)("nzShowIcon",!0)}}var rt=(()=>{class e{constructor(){this.router=c(R),this.settingsService=c(B),this.socialService=c(V),this.reuseTabService=c(Le,{optional:!0}),this.tokenService=c(pe),this.startupSrv=c(Te),this.http=c(ae),this.cdr=c(re),this.form=c(le).nonNullable.group({userName:["",[f.required]],password:["",[f.required]],mobile:["",[f.required]],captcha:["",[f.required]],remember:[!0]}),this.error="",this.type=0,this.loading=!1,this.count=0}switch({index:t}){this.type=t}getCaptcha(){let t=this.form.controls.mobile;if(t.invalid){t.markAsDirty({onlySelf:!0}),t.updateValueAndValidity({onlySelf:!0});return}this.count=59,this.interval$=setInterval(()=>{this.count-=1,this.count<=0&&clearInterval(this.interval$)},1e3)}submit(){if(this.error="",this.type===0){let{userName:t,password:i}=this.form.controls;if(t.markAsDirty(),t.updateValueAndValidity(),i.markAsDirty(),i.updateValueAndValidity(),t.invalid||i.invalid)return}else{let{mobile:t,captcha:i}=this.form.controls;if(t.markAsDirty(),t.updateValueAndValidity(),i.markAsDirty(),i.updateValueAndValidity(),t.invalid||i.invalid)return}this.loading=!0,this.cdr.detectChanges(),this.http.post("controlpanel/www?_allow_anonymous=true",{type:this.type,function:"Login",userName:this.form.value.userName,password:this.form.value.password},null,{context:new ne().set(se,!0)}).pipe(ee(()=>{this.loading=!1,this.cdr.detectChanges()})).subscribe(t=>{if(t.Status!==200){this.error=t.Message;return}this.reuseTabService?.clear(),t.Data.expired=+new Date+1e3*60*5,this.tokenService.set(t.Data),this.startupSrv.load().subscribe(()=>{let i=this.tokenService.referrer.url||"/";i.includes("/passport")&&(i="/"),this.router.navigateByUrl(i)})})}open(t,i="href"){let n="",u="";switch(ke.production?u=`https://ng-alain.github.io/ng-alain/#/passport/callback/${t}`:u=`http://localhost:4200/#/passport/callback/${t}`,t){case"auth0":n=`//cipchk.auth0.com/login?client=8gcNydIDzGBYxzqV0Vm1CX_RXH-wsWo5&redirect_uri=${decodeURIComponent(u)}`;break;case"github":n=`//github.com/login/oauth/authorize?client_id=9d6baae4b04a23fcafa2&response_type=code&redirect_uri=${decodeURIComponent(u)}`;break;case"weibo":n=`https://api.weibo.com/oauth2/authorize?client_id=1239507802&response_type=code&redirect_uri=${decodeURIComponent(u)}`;break}i==="window"?this.socialService.login(n,"/",{type:"window"}).subscribe(_=>{_&&(this.settingsService.setUser(_),this.router.navigateByUrl("/"))}):this.socialService.login(n,"/",{type:"href"})}ngOnDestroy(){this.interval$&&clearInterval(this.interval$)}static{this.\u0275fac=function(i){return new(i||e)}}static{this.\u0275cmp=S({type:e,selectors:[["passport-login"]],standalone:!0,features:[ie([V]),y],decls:26,vars:18,consts:[["nz-form","","role","form",3,"ngSubmit","formGroup"],[1,"tabs",3,"nzSelectChange","nzAnimated"],[3,"nzTitle"],[1,"mb-lg",3,"nzType","nzMessage","nzShowIcon"],["nzErrorTip","Please enter userName."],["nzSize","large","nzPrefixIcon","user"],["nz-input","","formControlName","userName","placeholder","username"],["nzErrorTip","Please enter password."],["nzSize","large","nzPrefixIcon","lock"],["nz-input","","type","password","formControlName","password","placeholder","password"],[3,"nzSpan"],["nz-checkbox","","formControlName","remember"],[1,"text-right",3,"nzSpan"],[1,"forgot"],["nz-button","","type","submit","nzType","primary","nzSize","large","nzBlock","",3,"nzLoading"]],template:function(i,n){i&1&&(o(0,"form",0),b("ngSubmit",function(){return n.submit()}),o(1,"nz-tabset",1),b("nzSelectChange",function(_){return n.switch(_)}),o(2,"nz-tab",2),s(3,"i18n"),z(4,mt,1,3,"nz-alert",3),o(5,"nz-form-item")(6,"nz-form-control",4)(7,"nz-input-group",5),g(8,"input",6),p()()(),o(9,"nz-form-item")(10,"nz-form-control",7)(11,"nz-input-group",8),g(12,"input",9),p()()()()(),o(13,"nz-form-item")(14,"nz-col",10)(15,"label",11),m(16),s(17,"i18n"),p()(),o(18,"nz-col",12)(19,"a",13),m(20),s(21,"i18n"),p()()(),o(22,"nz-form-item")(23,"button",14),m(24),s(25,"i18n"),p()()()),i&2&&(a("formGroup",n.form),r(),a("nzAnimated",!1),r(),a("nzTitle",l(3,10,"app.login.tab-login-credentials")),r(2),C(n.error?4:-1),r(10),a("nzSpan",12),r(2),v(l(17,12,"app.login.remember-me")),r(2),a("nzSpan",12),r(2),v(l(21,14,"app.login.forgot-password")),r(3),a("nzLoading",n.loading),r(),h(" ",l(25,16,"app.login.login")," "))},dependencies:[G,O,U,A,F,L,q,M,De,Pe,Oe,Fe,Ae,de,ce,Q,Y,H,X,K,J,j,$,W,T,I,x,k,Re,xe],styles:["[_nghost-%COMP%]{display:block;width:368px;margin:0 auto}[_nghost-%COMP%]     .ant-tabs .ant-tabs-bar{margin-bottom:24px;text-align:center;border-bottom:0}[_nghost-%COMP%]     .ant-tabs-tab{font-size:16px;line-height:24px}[_nghost-%COMP%]     .ant-input-affix-wrapper .ant-input:not(:first-child){padding-left:4px}[_nghost-%COMP%]     .icon{cursor:pointer;margin-left:16px;font-size:24px;color:#0003;vertical-align:middle;transition:color .3s}[_nghost-%COMP%]     .icon:hover{color:#1890ff}[_nghost-%COMP%]     .other{margin-top:24px;line-height:22px;text-align:left}[_nghost-%COMP%]     .other nz-tooltip{vertical-align:middle}[_nghost-%COMP%]     .other .register{float:right}[data-theme=dark]   [_nghost-%COMP%]     .icon{color:#fff3}[data-theme=dark]   [_nghost-%COMP%]     .icon:hover{color:#fff}"],changeDetection:0})}}return e})();function nt(e,d){return t=>{let i=t.get(e),n=t.get(d);return n.errors&&!n.errors.matchControl||(i.value!==n.value?n.setErrors({matchControl:!0}):n.setErrors(null)),null}}var ct=()=>({"width.px":240});function dt(e,d){if(e&1&&g(0,"nz-alert",6),e&2){let t=E();a("nzType","error")("nzMessage",t.error)("nzShowIcon",!0)}}function ut(e,d){e&1&&(m(0),s(1,"i18n")),e&2&&h(" ",l(1,1,"validation.email.required")," ")}function ft(e,d){e&1&&(m(0),s(1,"i18n")),e&2&&h(" ",l(1,1,"validation.email.wrong-format")," ")}function gt(e,d){if(e&1&&z(0,ut,2,3)(1,ft,2,3),e&2){let t=d.$implicit;C(t.errors!=null&&t.errors.required?0:-1),r(),C(t.errors!=null&&t.errors.email?1:-1)}}function zt(e,d){e&1&&(o(0,"div",24),m(1),s(2,"i18n"),p()),e&2&&(r(),v(l(2,1,"validation.password.strength.strong")))}function ht(e,d){e&1&&(o(0,"div",25),m(1),s(2,"i18n"),p()),e&2&&(r(),v(l(2,1,"validation.password.strength.medium")))}function vt(e,d){e&1&&(o(0,"div",26),m(1),s(2,"i18n"),p()),e&2&&(r(),v(l(2,1,"validation.password.strength.short")))}function Ct(e,d){if(e&1&&(o(0,"div",23),z(1,zt,3,3,"div",24)(2,ht,3,3,"div",25)(3,vt,3,3,"div",26),o(4,"div"),g(5,"nz-progress",27),p(),o(6,"p",28),m(7),s(8,"i18n"),p()()),e&2){let t,i=E();r(),C((t=i.status)==="ok"?1:t==="pass"?2:3),r(3),ve("progress-",i.status,""),r(),a("nzPercent",i.progress)("nzStatus",i.passwordProgressMap[i.status])("nzStrokeWidth",6)("nzShowInfo",!1),r(2),v(l(8,9,"validation.password.strength.msg"))}}function _t(e,d){e&1&&(m(0),s(1,"i18n")),e&2&&h(" ",l(1,1,"validation.confirm-password.required")," ")}function St(e,d){e&1&&(m(0),s(1,"i18n")),e&2&&h(" ",l(1,1,"validation.password.twice")," ")}function bt(e,d){if(e&1&&z(0,_t,2,3)(1,St,2,3),e&2){let t=d.$implicit;C(t.errors!=null&&t.errors.required?0:-1),r(),C(t.errors!=null&&t.errors.matchControl?1:-1)}}function yt(e,d){e&1&&(o(0,"nz-select",29),g(1,"nz-option",30)(2,"nz-option",30),p()),e&2&&(r(),a("nzLabel","+86")("nzValue","+86"),r(),a("nzLabel","+87")("nzValue","+87"))}function Nt(e,d){e&1&&(m(0),s(1,"i18n")),e&2&&h(" ",l(1,1,"validation.phone-number.required")," ")}function wt(e,d){e&1&&(m(0),s(1,"i18n")),e&2&&h(" ",l(1,1,"validation.phone-number.wrong-format")," ")}function xt(e,d){if(e&1&&z(0,Nt,2,3)(1,wt,2,3),e&2){let t=d.$implicit;C(t.errors!=null&&t.errors.required?0:-1),r(),C(t.errors!=null&&t.errors.pattern?1:-1)}}var ot=(()=>{class e{constructor(){this.router=c(R),this.http=c(ae),this.cdr=c(re),this.form=c(le).nonNullable.group({mail:["",[f.required,f.email]],password:["",[f.required,f.minLength(6),e.checkPassword.bind(this)]],confirm:["",[f.required,f.minLength(6)]],mobilePrefix:["+86"],mobile:["",[f.required,f.pattern(/^1\d{10}$/)]],captcha:["",[f.required]]},{validators:nt("password","confirm")}),this.error="",this.type=0,this.loading=!1,this.visible=!1,this.status="pool",this.progress=0,this.passwordProgressMap={ok:"success",pass:"normal",pool:"exception"},this.count=0}static checkPassword(t){if(!t)return null;let i=this;i.visible=!!t.value,t.value&&t.value.length>9?i.status="ok":t.value&&t.value.length>5?i.status="pass":i.status="pool",i.visible&&(i.progress=t.value.length*10>100?100:t.value.length*10)}getCaptcha(){let{mobile:t}=this.form.controls;if(t.invalid){t.markAsDirty({onlySelf:!0}),t.updateValueAndValidity({onlySelf:!0});return}this.count=59,this.cdr.detectChanges(),this.interval$=setInterval(()=>{this.count-=1,this.cdr.detectChanges(),this.count<=0&&clearInterval(this.interval$)},1e3)}submit(){if(this.error="",Object.keys(this.form.controls).forEach(i=>{let n=this.form.controls[i];n.markAsDirty(),n.updateValueAndValidity()}),this.form.invalid)return;let t=this.form.value;this.loading=!0,this.cdr.detectChanges(),this.http.post("/register",t,null,{context:new ne().set(se,!0)}).pipe(ee(()=>{this.loading=!1,this.cdr.detectChanges()})).subscribe(()=>{this.router.navigate(["passport","register-result"],{queryParams:{email:t.mail}})})}ngOnDestroy(){this.interval$&&clearInterval(this.interval$)}static{this.\u0275fac=function(i){return new(i||e)}}static{this.\u0275cmp=S({type:e,selectors:[["passport-register"]],standalone:!0,features:[y],decls:50,vars:34,consts:[["mailErrorTip",""],["pwdCdkTpl",""],["confirmErrorTip",""],["addOnBeforeTemplate",""],["mobileErrorTip",""],["nz-form","","role","form",3,"ngSubmit","formGroup"],[1,"mb-lg",3,"nzType","nzMessage","nzShowIcon"],[3,"nzErrorTip"],["nzSize","large","nzAddonBeforeIcon","user"],["nz-input","","formControlName","mail","placeholder","Email"],["nzSize","large","nzAddonBeforeIcon","lock","nz-popover","","nzPopoverPlacement","right","nzPopoverTrigger","focus","nzPopoverOverlayClassName","register-password-cdk",3,"nzPopoverVisibleChange","nzPopoverVisible","nzPopoverOverlayStyle","nzPopoverContent"],["nz-input","","type","password","formControlName","password","placeholder","Password"],["nzSize","large","nzAddonBeforeIcon","lock"],["nz-input","","type","password","formControlName","confirm","placeholder","Confirm Password"],["nzSize","large",3,"nzAddOnBefore"],["formControlName","mobile","nz-input","","placeholder","Phone number"],["nz-row","",3,"nzGutter"],["nz-col","",3,"nzSpan"],["nzSize","large","nzAddonBeforeIcon","mail"],["nz-input","","formControlName","captcha","placeholder","Captcha"],["type","button","nz-button","","nzSize","large","nzBlock","",3,"click","disabled","nzLoading"],["nz-button","","nzType","primary","nzSize","large","type","submit",1,"submit",3,"nzLoading"],["routerLink","/passport/login",1,"login"],[2,"padding","4px 0"],[1,"success"],[1,"warning"],[1,"error"],[3,"nzPercent","nzStatus","nzStrokeWidth","nzShowInfo"],[1,"mt-sm"],["formControlName","mobilePrefix",2,"width","100px"],[3,"nzLabel","nzValue"]],template:function(i,n){if(i&1){let u=te();o(0,"h3"),m(1),s(2,"i18n"),p(),o(3,"form",5),b("ngSubmit",function(){return P(u),D(n.submit())}),z(4,dt,1,3,"nz-alert",6),o(5,"nz-form-item")(6,"nz-form-control",7)(7,"nz-input-group",8),g(8,"input",9),p(),z(9,gt,2,2,"ng-template",null,0,w),p()(),o(11,"nz-form-item")(12,"nz-form-control",7),s(13,"i18n"),o(14,"nz-input-group",10),be("nzPopoverVisibleChange",function(Z){return P(u),Se(n.visible,Z)||(n.visible=Z),D(Z)}),g(15,"input",11),p(),z(16,Ct,9,11,"ng-template",null,1,w),p()(),o(18,"nz-form-item")(19,"nz-form-control",7)(20,"nz-input-group",12),g(21,"input",13),p(),z(22,bt,2,2,"ng-template",null,2,w),p()(),o(24,"nz-form-item")(25,"nz-form-control",7)(26,"nz-input-group",14),z(27,yt,3,4,"ng-template",null,3,w),g(29,"input",15),p(),z(30,xt,2,2,"ng-template",null,4,w),p()(),o(32,"nz-form-item")(33,"nz-form-control",7),s(34,"i18n"),o(35,"div",16)(36,"div",17)(37,"nz-input-group",18),g(38,"input",19),p()(),o(39,"div",17)(40,"button",20),b("click",function(){return P(u),D(n.getCaptcha())}),m(41),s(42,"i18n"),p()()()()(),o(43,"nz-form-item")(44,"button",21),m(45),s(46,"i18n"),p(),o(47,"a",22),m(48),s(49,"i18n"),p()()()}if(i&2){let u=N(10),_=N(17),Z=N(23),pt=N(28),st=N(31);r(),v(l(2,21,"app.register.register")),r(2),a("formGroup",n.form),r(),C(n.error?4:-1),r(2),a("nzErrorTip",u),r(6),a("nzErrorTip",l(13,23,"validation.password.required")),r(2),_e("nzPopoverVisible",n.visible),a("nzPopoverOverlayStyle",ye(33,ct))("nzPopoverContent",_),r(5),a("nzErrorTip",Z),r(6),a("nzErrorTip",st),r(),a("nzAddOnBefore",pt),r(7),a("nzErrorTip",l(34,25,"validation.verification-code.required")),r(2),a("nzGutter",8),r(),a("nzSpan",16),r(3),a("nzSpan",8),r(),a("disabled",n.count>0)("nzLoading",n.loading),r(),h(" ",n.count?n.count+"s":l(42,27,"app.register.get-verification-code")," "),r(3),a("nzLoading",n.loading),r(),h(" ",l(46,29,"app.register.register")," "),r(3),v(l(49,31,"app.register.sign-in"))}},dependencies:[G,O,U,A,F,L,q,M,oe,de,ce,Q,Y,H,X,K,J,j,$,W,We,$e,He,je,Ue,Be,Ve,me,T,I,x,k],styles:["[_nghost-%COMP%]{display:block;width:368px;margin:0 auto}[_nghost-%COMP%]     h3{margin-bottom:20px;font-size:16px}[_nghost-%COMP%]     .submit{width:50%}[_nghost-%COMP%]     .login{float:right;line-height:40px}  .register-password-cdk .success,   .register-password-cdk .warning,   .register-password-cdk .error{transition:color .3s}  .register-password-cdk .success{color:#52c41a}  .register-password-cdk .warning{color:#faad14}  .register-password-cdk .error{color:#ff4d4f}  .register-password-cdk .progress-pass>.progress .ant-progress-bg{background-color:#faad14}"],changeDetection:0})}}return e})();var kt=e=>({email:e});function It(e,d){if(e&1&&(o(0,"div",4),m(1),s(2,"i18n"),p()),e&2){let t=E();r(),h(" ",we(2,1,"app.register-result.msg",Ne(4,kt,t.email))," ")}}var at=(()=>{class e{constructor(){this.msg=c(Ie),this.email=""}static{this.\u0275fac=function(i){return new(i||e)}}static{this.\u0275cmp=S({type:e,selectors:[["passport-register-result"]],inputs:{email:"email"},standalone:!0,features:[y],decls:10,vars:11,consts:[["title",""],["type","success",3,"title","description"],["nz-button","","nzSize","large",3,"click","nzType"],["routerLink","/","nz-button","","nzSize","large"],[1,"title",2,"font-size","20px"]],template:function(i,n){if(i&1){let u=te();o(0,"result",1),s(1,"i18n"),z(2,It,3,6,"ng-template",null,0,w),o(4,"button",2),b("click",function(){return P(u),D(n.msg.success("email"))}),m(5),s(6,"i18n"),p(),o(7,"button",3),m(8),s(9,"i18n"),p()()}if(i&2){let u=N(3);Ce("description",l(1,5,"app.register-result.activation-email")),a("title",u),r(4),a("nzType","primary"),r(),h(" ",l(6,7,"app.register-result.view-mailbox")," "),r(3),h(" ",l(9,9,"app.register-result.back-home")," ")}},dependencies:[oe,M,T,I,x,k,Xe,Ke],encapsulation:2})}}return e})();var nr=[{path:"passport",component:Ye,children:[{path:"login",component:rt,data:{title:"\u767B\u5F55",titleI18n:"app.login.login"}},{path:"register",component:ot,data:{title:"\u6CE8\u518C",titleI18n:"app.register.register"}},{path:"register-result",component:at,data:{title:"\u6CE8\u518C\u7ED3\u679C",titleI18n:"app.register.register"}},{path:"lock",component:it,data:{title:"\u9501\u5C4F",titleI18n:"app.lock"}}]},{path:"passport/callback/:type",component:Je}];export{nr as routes};