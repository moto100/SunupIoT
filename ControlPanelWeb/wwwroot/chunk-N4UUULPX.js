import{a as Se,h as ye,l as qe}from"./chunk-OCYLFZI7.js";import{a as ne,b as B,c as oe}from"./chunk-ON5T7QZJ.js";import{a as Ge,b as $e}from"./chunk-YXWFCQY5.js";import{$ as we,A as xe,C as O,Ca as Te,Da as Me,E as V,Ea as De,H as L,Ia as G,Ka as $,L as ae,N as q,Na as W,Sa as j,Ta as H,Ua as pe,Va as Y,Wa as K,Xa as X,Za as J,ab as Pe,ba as ke,bb as Ee,cb as Re,db as Be,eb as se,fb as le,gb as Ae,hb as Fe,ib as Ue,ja as Ie,jb as Oe,kb as Ve,lb as Le,s as be,u as A,v as f,w as F,x as U,y as Ne}from"./chunk-HCTXQRD3.js";import{$a as s,Bb as l,Ca as r,Cb as c,Db as Ce,Ea as Z,La as a,Lb as w,Lc as E,Mc as ie,P as Q,Qa as ge,Ua as h,Va as _,_a as o,a as ce,ab as z,da as d,ea as b,ib as S,kb as P,mf as _e,nb as ze,nf as k,oc as te,of as I,pb as x,pg as R,qb as u,qf as T,rb as C,sb as v,sf as M,sg as D,ub as ee,vb as y,vg as re,wb as he,xb as ve}from"./chunk-7CJ5GJTD.js";var We=(()=>{let t=class t{constructor(){this.socialService=d(B),this.settingsSrv=d(R),this.type=""}ngOnInit(){this.mockModel()}mockModel(){let i={token:"123456789",name:"cipchk",email:`${this.type}@${this.type}.com`,id:1e4,time:+new Date};this.settingsSrv.setUser(ce(ce({},this.settingsSrv.user),i)),this.socialService.callback(i)}};t.\u0275fac=function(n){return new(n||t)},t.\u0275cmp=b({type:t,selectors:[["app-callback"]],inputs:{type:"type"},standalone:!0,features:[ee([B]),y],decls:0,vars:0,template:function(n,p){},encapsulation:2});let e=t;return e})();var Xe=(()=>{let t=class t{constructor(){this.tokenService=d(ne),this.settings=d(R),this.router=d(E),this.f=new Ne({password:new xe("",{nonNullable:!0,validators:[f.required]})})}get user(){return this.settings.user}submit(){this.f.controls.password.markAsDirty(),this.f.controls.password.updateValueAndValidity(),this.f.valid&&(console.log("Valid!"),console.log(this.f.value),this.tokenService.set({token:"123"}),this.router.navigate(["dashboard"]))}};t.\u0275fac=function(n){return new(n||t)},t.\u0275cmp=b({type:t,selectors:[["passport-lock"]],standalone:!0,features:[y],decls:15,vars:11,consts:[[1,"ant-card","width-lg",2,"margin","0 auto"],[1,"ant-card-body"],[1,"avatar"],["nzIcon","user","nzSize","large",3,"nzSrc"],["nz-form","","role","form",1,"mt-md",3,"formGroup","ngSubmit"],[3,"nzErrorTip"],["nzSuffixIcon","lock"],["type","password","nz-input","","formControlName","password"],["nz-row","","nzType","flex","nzAlign","middle"],["nz-col","",2,"text-align","right",3,"nzOffset","nzSpan"],["nz-button","","nzType","primary",3,"disabled"]],template:function(n,p){n&1&&(o(0,"div",0)(1,"div",1)(2,"div",2),z(3,"nz-avatar",3),s(),o(4,"form",4),S("ngSubmit",function(){return p.submit()}),o(5,"nz-form-item")(6,"nz-form-control",5),l(7,"i18n"),o(8,"nz-input-group",6),z(9,"input",7),s()()(),o(10,"div",8)(11,"div",9)(12,"button",10),u(13),l(14,"i18n"),s()()()()()()),n&2&&(r(3),a("nzSrc",p.user.avatar),r(1),a("formGroup",p.f),r(2),a("nzErrorTip",c(7,7,"validation.password.required")),r(5),a("nzOffset",12)("nzSpan",12),r(1),a("disabled",!p.f.valid),r(1),C(c(14,9,"app.lock")))},dependencies:[q,O,A,F,U,V,L,D,Fe,Ae,J,H,j,K,Y,X,pe,M,T,k,I,W,G,$],styles:["[_nghost-%COMP%]     .ant-card-body{position:relative;margin-top:80px}[_nghost-%COMP%]     .avatar{position:absolute;top:-20px;left:50%;margin-left:-20px}"]});let e=t;return e})();function ot(e,t){if(e&1&&z(0,"nz-alert",15),e&2){let m=P();a("nzType","error")("nzMessage",m.error)("nzShowIcon",!0)}}var Je=(()=>{let t=class t{constructor(){this.router=d(E),this.settingsService=d(R),this.socialService=d(B),this.reuseTabService=d(Be,{optional:!0}),this.tokenService=d(ne),this.startupSrv=d(ye),this.http=d(re),this.cdr=d(Z),this.form=d(ae).nonNullable.group({userName:["",[f.required]],password:["",[f.required]],mobile:["",[f.required]],captcha:["",[f.required]],remember:[!0]}),this.error="",this.type=0,this.loading=!1,this.count=0}switch({index:i}){this.type=i}getCaptcha(){let i=this.form.controls.mobile;if(i.invalid){i.markAsDirty({onlySelf:!0}),i.updateValueAndValidity({onlySelf:!0});return}this.count=59,this.interval$=setInterval(()=>{this.count-=1,this.count<=0&&clearInterval(this.interval$)},1e3)}submit(){if(this.error="",this.type===0){let{userName:i,password:n}=this.form.controls;if(i.markAsDirty(),i.updateValueAndValidity(),n.markAsDirty(),n.updateValueAndValidity(),i.invalid||n.invalid)return}else{let{mobile:i,captcha:n}=this.form.controls;if(i.markAsDirty(),i.updateValueAndValidity(),n.markAsDirty(),n.updateValueAndValidity(),i.invalid||n.invalid)return}this.loading=!0,this.cdr.detectChanges(),this.http.post("controlpanel/www?_allow_anonymous=true",{type:this.type,function:"Login",userName:this.form.value.userName,password:this.form.value.password},null,{context:new te().set(oe,!0)}).pipe(Q(()=>{this.loading=!1,this.cdr.detectChanges()})).subscribe(i=>{if(i.Status!==200){this.error=i.Message;return}this.reuseTabService?.clear(),i.Data.expired=+new Date+1e3*60*5,this.tokenService.set(i.Data),this.startupSrv.load().subscribe(()=>{let n=this.tokenService.referrer.url||"/";n.includes("/passport")&&(n="/"),this.router.navigateByUrl(n)})})}open(i,n="href"){let p="",g="";switch(Se.production?g=`https://ng-alain.github.io/ng-alain/#/passport/callback/${i}`:g=`http://localhost:4200/#/passport/callback/${i}`,i){case"auth0":p=`//cipchk.auth0.com/login?client=8gcNydIDzGBYxzqV0Vm1CX_RXH-wsWo5&redirect_uri=${decodeURIComponent(g)}`;break;case"github":p=`//github.com/login/oauth/authorize?client_id=9d6baae4b04a23fcafa2&response_type=code&redirect_uri=${decodeURIComponent(g)}`;break;case"weibo":p=`https://api.weibo.com/oauth2/authorize?client_id=1239507802&response_type=code&redirect_uri=${decodeURIComponent(g)}`;break}n==="window"?this.socialService.login(p,"/",{type:"window"}).subscribe(N=>{N&&(this.settingsService.setUser(N),this.router.navigateByUrl("/"))}):this.socialService.login(p,"/",{type:"href"})}ngOnDestroy(){this.interval$&&clearInterval(this.interval$)}};t.\u0275fac=function(n){return new(n||t)},t.\u0275cmp=b({type:t,selectors:[["passport-login"]],standalone:!0,features:[ee([B]),y],decls:26,vars:18,consts:[["nz-form","","role","form",3,"formGroup","ngSubmit"],[1,"tabs",3,"nzAnimated","nzSelectChange"],[3,"nzTitle"],["class","mb-lg",3,"nzType","nzMessage","nzShowIcon"],["nzErrorTip","Please enter userName."],["nzSize","large","nzPrefixIcon","user"],["nz-input","","formControlName","userName","placeholder","username"],["nzErrorTip","Please enter password."],["nzSize","large","nzPrefixIcon","lock"],["nz-input","","type","password","formControlName","password","placeholder","password"],[3,"nzSpan"],["nz-checkbox","","formControlName","remember"],[1,"text-right",3,"nzSpan"],[1,"forgot"],["nz-button","","type","submit","nzType","primary","nzSize","large","nzBlock","",3,"nzLoading"],[1,"mb-lg",3,"nzType","nzMessage","nzShowIcon"]],template:function(n,p){n&1&&(o(0,"form",0),S("ngSubmit",function(){return p.submit()}),o(1,"nz-tabset",1),S("nzSelectChange",function(N){return p.switch(N)}),o(2,"nz-tab",2),l(3,"i18n"),h(4,ot,1,3,"nz-alert",3),o(5,"nz-form-item")(6,"nz-form-control",4)(7,"nz-input-group",5),z(8,"input",6),s()()(),o(9,"nz-form-item")(10,"nz-form-control",7)(11,"nz-input-group",8),z(12,"input",9),s()()()()(),o(13,"nz-form-item")(14,"nz-col",10)(15,"label",11),u(16),l(17,"i18n"),s()(),o(18,"nz-col",12)(19,"a",13),u(20),l(21,"i18n"),s()()(),o(22,"nz-form-item")(23,"button",14),u(24),l(25,"i18n"),s()()()),n&2&&(a("formGroup",p.form),r(1),a("nzAnimated",!1),r(1),a("nzTitle",c(3,10,"app.login.tab-login-credentials")),r(2),_(4,p.error?4:-1),r(10),a("nzSpan",12),r(2),C(c(17,12,"app.login.remember-me")),r(2),a("nzSpan",12),r(2),C(c(21,14,"app.login.forgot-password")),r(3),a("nzLoading",p.loading),r(1),v(" ",c(25,16,"app.login.login")," "))},dependencies:[q,O,A,F,U,V,L,D,ke,we,Re,Ee,Pe,le,se,J,H,j,K,Y,X,W,G,$,M,T,k,I,Ie,_e],styles:["[_nghost-%COMP%]{display:block;width:368px;margin:0 auto}[_nghost-%COMP%]     .ant-tabs .ant-tabs-bar{margin-bottom:24px;text-align:center;border-bottom:0}[_nghost-%COMP%]     .ant-tabs-tab{font-size:16px;line-height:24px}[_nghost-%COMP%]     .ant-input-affix-wrapper .ant-input:not(:first-child){padding-left:4px}[_nghost-%COMP%]     .icon{cursor:pointer;margin-left:16px;font-size:24px;color:#0003;vertical-align:middle;transition:color .3s}[_nghost-%COMP%]     .icon:hover{color:#1890ff}[_nghost-%COMP%]     .other{margin-top:24px;line-height:22px;text-align:left}[_nghost-%COMP%]     .other nz-tooltip{vertical-align:middle}[_nghost-%COMP%]     .other .register{float:right}[data-theme=dark]   [_nghost-%COMP%]     .icon{color:#fff3}[data-theme=dark]   [_nghost-%COMP%]     .icon:hover{color:#fff}"],changeDetection:0});let e=t;return e})();function Qe(e,t){return m=>{let i=m.get(e),n=m.get(t);return n.errors&&!n.errors.matchControl||(i.value!==n.value?n.setErrors({matchControl:!0}):n.setErrors(null)),null}}function at(e,t){if(e&1&&z(0,"nz-alert",23),e&2){let m=P();a("nzType","error")("nzMessage",m.error)("nzShowIcon",!0)}}function pt(e,t){e&1&&(u(0),l(1,"i18n")),e&2&&v(" ",c(1,1,"validation.email.required")," ")}function st(e,t){e&1&&(u(0),l(1,"i18n")),e&2&&v(" ",c(1,1,"validation.email.wrong-format")," ")}function lt(e,t){if(e&1&&h(0,pt,2,3)(1,st,2,3),e&2){let m=t.$implicit;_(0,m.errors!=null&&m.errors.required?0:-1),r(1),_(1,m.errors!=null&&m.errors.email?1:-1)}}function mt(e,t){e&1&&(o(0,"div",27),u(1),l(2,"i18n"),s()),e&2&&(r(1),C(c(2,1,"validation.password.strength.strong")))}function ct(e,t){e&1&&(o(0,"div",28),u(1),l(2,"i18n"),s()),e&2&&(r(1),C(c(2,1,"validation.password.strength.medium")))}function ut(e,t){e&1&&(o(0,"div",29),u(1),l(2,"i18n"),s()),e&2&&(r(1),C(c(2,1,"validation.password.strength.short")))}function dt(e,t){if(e&1&&(o(0,"div",24),h(1,mt,3,3)(2,ct,3,3)(3,ut,3,3),o(4,"div"),z(5,"nz-progress",25),s(),o(6,"p",26),u(7),l(8,"i18n"),s()()),e&2){let m=P(),i;r(1),_(1,(i=m.status)==="ok"?1:i==="pass"?2:3),r(3),ge("progress-",m.status,""),r(1),a("nzPercent",m.progress)("nzStatus",m.passwordProgressMap[m.status])("nzStrokeWidth",6)("nzShowInfo",!1),r(2),C(c(8,9,"validation.password.strength.msg"))}}function ft(e,t){e&1&&(u(0),l(1,"i18n")),e&2&&v(" ",c(1,1,"validation.confirm-password.required")," ")}function gt(e,t){e&1&&(u(0),l(1,"i18n")),e&2&&v(" ",c(1,1,"validation.password.twice")," ")}function zt(e,t){if(e&1&&h(0,ft,2,3)(1,gt,2,3),e&2){let m=t.$implicit;_(0,m.errors!=null&&m.errors.required?0:-1),r(1),_(1,m.errors!=null&&m.errors.matchControl?1:-1)}}function ht(e,t){e&1&&(o(0,"nz-select",30),z(1,"nz-option",31)(2,"nz-option",31),s()),e&2&&(r(1),a("nzLabel","+86")("nzValue","+86"),r(1),a("nzLabel","+87")("nzValue","+87"))}function vt(e,t){e&1&&(u(0),l(1,"i18n")),e&2&&v(" ",c(1,1,"validation.phone-number.required")," ")}function Ct(e,t){e&1&&(u(0),l(1,"i18n")),e&2&&v(" ",c(1,1,"validation.phone-number.wrong-format")," ")}function _t(e,t){if(e&1&&h(0,vt,2,3)(1,Ct,2,3),e&2){let m=t.$implicit;_(0,m.errors!=null&&m.errors.required?0:-1),r(1),_(1,m.errors!=null&&m.errors.pattern?1:-1)}}var St=()=>({"width.px":240}),Ze=(()=>{let t=class t{constructor(){this.router=d(E),this.http=d(re),this.cdr=d(Z),this.form=d(ae).nonNullable.group({mail:["",[f.required,f.email]],password:["",[f.required,f.minLength(6),t.checkPassword.bind(this)]],confirm:["",[f.required,f.minLength(6)]],mobilePrefix:["+86"],mobile:["",[f.required,f.pattern(/^1\d{10}$/)]],captcha:["",[f.required]]},{validators:Qe("password","confirm")}),this.error="",this.type=0,this.loading=!1,this.visible=!1,this.status="pool",this.progress=0,this.passwordProgressMap={ok:"success",pass:"normal",pool:"exception"},this.count=0}static checkPassword(i){if(!i)return null;let n=this;n.visible=!!i.value,i.value&&i.value.length>9?n.status="ok":i.value&&i.value.length>5?n.status="pass":n.status="pool",n.visible&&(n.progress=i.value.length*10>100?100:i.value.length*10)}getCaptcha(){let{mobile:i}=this.form.controls;if(i.invalid){i.markAsDirty({onlySelf:!0}),i.updateValueAndValidity({onlySelf:!0});return}this.count=59,this.cdr.detectChanges(),this.interval$=setInterval(()=>{this.count-=1,this.cdr.detectChanges(),this.count<=0&&clearInterval(this.interval$)},1e3)}submit(){if(this.error="",Object.keys(this.form.controls).forEach(n=>{let p=this.form.controls[n];p.markAsDirty(),p.updateValueAndValidity()}),this.form.invalid)return;let i=this.form.value;this.loading=!0,this.cdr.detectChanges(),this.http.post("/register",i,null,{context:new te().set(oe,!0)}).pipe(Q(()=>{this.loading=!1,this.cdr.detectChanges()})).subscribe(()=>{this.router.navigate(["passport","register-result"],{queryParams:{email:i.mail}})})}ngOnDestroy(){this.interval$&&clearInterval(this.interval$)}};t.\u0275fac=function(n){return new(n||t)},t.\u0275cmp=b({type:t,selectors:[["passport-register"]],standalone:!0,features:[y],decls:50,vars:34,consts:[["nz-form","","role","form",3,"formGroup","ngSubmit"],["class","mb-lg",3,"nzType","nzMessage","nzShowIcon"],[3,"nzErrorTip"],["nzSize","large","nzAddonBeforeIcon","user"],["nz-input","","formControlName","mail","placeholder","Email"],["mailErrorTip",""],["nzSize","large","nzAddonBeforeIcon","lock","nz-popover","","nzPopoverPlacement","right","nzPopoverTrigger","focus","nzPopoverOverlayClassName","register-password-cdk",3,"nzPopoverVisible","nzPopoverOverlayStyle","nzPopoverContent","nzPopoverVisibleChange"],["nz-input","","type","password","formControlName","password","placeholder","Password"],["pwdCdkTpl",""],["nzSize","large","nzAddonBeforeIcon","lock"],["nz-input","","type","password","formControlName","confirm","placeholder","Confirm Password"],["confirmErrorTip",""],["nzSize","large",3,"nzAddOnBefore"],["addOnBeforeTemplate",""],["formControlName","mobile","nz-input","","placeholder","Phone number"],["mobileErrorTip",""],["nz-row","",3,"nzGutter"],["nz-col","",3,"nzSpan"],["nzSize","large","nzAddonBeforeIcon","mail"],["nz-input","","formControlName","captcha","placeholder","Captcha"],["type","button","nz-button","","nzSize","large","nzBlock","",3,"disabled","nzLoading","click"],["nz-button","","nzType","primary","nzSize","large","type","submit",1,"submit",3,"nzLoading"],["routerLink","/passport/login",1,"login"],[1,"mb-lg",3,"nzType","nzMessage","nzShowIcon"],[2,"padding","4px 0"],[3,"nzPercent","nzStatus","nzStrokeWidth","nzShowInfo"],[1,"mt-sm"],[1,"success"],[1,"warning"],[1,"error"],["formControlName","mobilePrefix",2,"width","100px"],[3,"nzLabel","nzValue"]],template:function(n,p){if(n&1&&(o(0,"h3"),u(1),l(2,"i18n"),s(),o(3,"form",0),S("ngSubmit",function(){return p.submit()}),h(4,at,1,3,"nz-alert",1),o(5,"nz-form-item")(6,"nz-form-control",2)(7,"nz-input-group",3),z(8,"input",4),s(),h(9,lt,2,2,"ng-template",null,5,w),s()(),o(11,"nz-form-item")(12,"nz-form-control",2),l(13,"i18n"),o(14,"nz-input-group",6),S("nzPopoverVisibleChange",function(N){return p.visible=N}),z(15,"input",7),s(),h(16,dt,9,11,"ng-template",null,8,w),s()(),o(18,"nz-form-item")(19,"nz-form-control",2)(20,"nz-input-group",9),z(21,"input",10),s(),h(22,zt,2,2,"ng-template",null,11,w),s()(),o(24,"nz-form-item")(25,"nz-form-control",2)(26,"nz-input-group",12),h(27,ht,3,4,"ng-template",null,13,w),z(29,"input",14),s(),h(30,_t,2,2,"ng-template",null,15,w),s()(),o(32,"nz-form-item")(33,"nz-form-control",2),l(34,"i18n"),o(35,"div",16)(36,"div",17)(37,"nz-input-group",18),z(38,"input",19),s()(),o(39,"div",17)(40,"button",20),S("click",function(){return p.getCaptcha()}),u(41),l(42,"i18n"),s()()()()(),o(43,"nz-form-item")(44,"button",21),u(45),l(46,"i18n"),s(),o(47,"a",22),u(48),l(49,"i18n"),s()()()),n&2){let g=x(10),N=x(17),tt=x(23),it=x(28),rt=x(31);r(1),C(c(2,21,"app.register.register")),r(2),a("formGroup",p.form),r(1),_(4,p.error?4:-1),r(2),a("nzErrorTip",g),r(6),a("nzErrorTip",c(13,23,"validation.password.required")),r(2),a("nzPopoverVisible",p.visible)("nzPopoverOverlayStyle",he(33,St))("nzPopoverContent",N),r(5),a("nzErrorTip",tt),r(6),a("nzErrorTip",rt),r(1),a("nzAddOnBefore",it),r(7),a("nzErrorTip",c(34,25,"validation.verification-code.required")),r(2),a("nzGutter",8),r(1),a("nzSpan",16),r(3),a("nzSpan",8),r(1),a("disabled",p.count>0)("nzLoading",p.loading),r(1),v(" ",p.count?p.count+"s":c(42,27,"app.register.get-verification-code")," "),r(3),a("nzLoading",p.loading),r(1),v(" ",c(46,29,"app.register.register")," "),r(3),C(c(49,31,"app.register.sign-in"))}},dependencies:[q,O,A,F,U,V,L,D,ie,le,se,J,H,j,K,Y,X,W,G,$,Oe,Ue,Le,Ve,De,Te,Me,pe,M,T,k,I],styles:["[_nghost-%COMP%]{display:block;width:368px;margin:0 auto}[_nghost-%COMP%]     h3{margin-bottom:20px;font-size:16px}[_nghost-%COMP%]     .submit{width:50%}[_nghost-%COMP%]     .login{float:right;line-height:40px}  .register-password-cdk .success,   .register-password-cdk .warning,   .register-password-cdk .error{transition:color .3s}  .register-password-cdk .success{color:#52c41a}  .register-password-cdk .warning{color:#faad14}  .register-password-cdk .error{color:#ff4d4f}  .register-password-cdk .progress-pass>.progress .ant-progress-bg{background-color:#faad14}"],changeDetection:0});let e=t;return e})();var bt=e=>({email:e});function yt(e,t){if(e&1&&(o(0,"div",4),u(1),l(2,"i18n"),s()),e&2){let m=P();r(1),v(" ",Ce(2,1,"app.register-result.msg",ve(4,bt,m.email))," ")}}var et=(()=>{let t=class t{constructor(){this.msg=d(be),this.email=""}};t.\u0275fac=function(n){return new(n||t)},t.\u0275cmp=b({type:t,selectors:[["passport-register-result"]],inputs:{email:"email"},standalone:!0,features:[y],decls:10,vars:11,consts:[["type","success",3,"title","description"],["title",""],["nz-button","","nzSize","large",3,"nzType","click"],["routerLink","/","nz-button","","nzSize","large"],[1,"title",2,"font-size","20px"]],template:function(n,p){if(n&1&&(o(0,"result",0),l(1,"i18n"),h(2,yt,3,6,"ng-template",null,1,w),o(4,"button",2),S("click",function(){return p.msg.success("email")}),u(5),l(6,"i18n"),s(),o(7,"button",3),u(8),l(9,"i18n"),s()()),n&2){let g=x(3);ze("description",c(1,5,"app.register-result.activation-email")),a("title",g),r(4),a("nzType","primary"),r(1),v(" ",c(6,7,"app.register-result.view-mailbox")," "),r(3),v(" ",c(9,9,"app.register-result.back-home")," ")}},dependencies:[ie,D,M,T,k,I,$e,Ge],encapsulation:2});let e=t;return e})();var Zi=[{path:"passport",component:qe,children:[{path:"login",component:Je,data:{title:"\u767B\u5F55",titleI18n:"app.login.login"}},{path:"register",component:Ze,data:{title:"\u6CE8\u518C",titleI18n:"app.register.register"}},{path:"register-result",component:et,data:{title:"\u6CE8\u518C\u7ED3\u679C",titleI18n:"app.register.register"}},{path:"lock",component:Xe,data:{title:"\u9501\u5C4F",titleI18n:"app.lock"}}]},{path:"passport/callback/:type",component:We}];export{Zi as routes};
