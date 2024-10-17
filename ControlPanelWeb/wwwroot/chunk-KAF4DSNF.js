import{M as at,O as ct}from"./chunk-EA4M7BGS.js";import{$ as A,$a as U,E as Q,Ea as P,Fa as m,Gb as d,Ha as p,Nd as rt,Ob as tt,Od as pt,Pb as D,Qa as F,Qb as k,V,Va as x,Vb as et,Wa as w,Xa as T,Y as g,Zb as nt,_ as W,_a as N,ac as it,bc as lt,cb as G,ea as j,eb as a,fa as Z,fb as J,ga as q,gb as K,gc as ot,kb as O,lb as S,mb as b,na as $,nb as _,oa as B,ob as M,pb as L,sa as R,vc as st,wb as X,xa as C,xb as Y,yb as E}from"./chunk-MUNQRYA4.js";var ft=["orgEl"],gt=["shadowOrgEl"],Ct=["shadowTextEl"],dt=["*"],xt=()=>({"overflow-wrap":"break-word","word-wrap":"break-word"}),z=e=>({$implicit:e}),Tt=e=>({"-webkit-line-clamp":e,"-webkit-box-orient":"vertical"});function Et(e,s){e&1&&N(0)}function yt(e,s){if(e&1&&T(0,"div",12),e&2){let t=a(3);p("innerHTML",t.orgHtml,R)}}function vt(e,s){if(e&1&&(x(0,"span",10),m(1,Et,1,0,"ng-container",11)(2,yt,1,1,"ng-template",null,2,d),w()),e&2){let t=_(3),n=a().$implicit;p("nzTooltipTitle",t)("nzTooltipOverlayStyle",Y(3,xt)),C(),p("ngTemplateOutlet",n)}}function wt(e,s){e&1&&N(0)}function Ot(e,s){if(e&1&&m(0,wt,1,0,"ng-container",11),e&2){let t=a().$implicit;p("ngTemplateOutlet",t)}}function St(e,s){if(e&1&&m(0,vt,4,4,"span",10)(1,Ot,1,1,"ng-container"),e&2){let t=a();F(t.tooltip?0:1)}}function bt(e,s){if(e&1&&T(0,"span",9),e&2){let t=a();p("ngClass",t.cls)}}function Mt(e,s){}function Lt(e,s){if(e&1&&M(0),e&2){let t=a(2);L(t.text)}}function Ht(e,s){if(e&1&&m(0,Mt,0,0,"ng-template",13)(1,Lt,1,1,"ng-template",null,3,d),e&2){let t=_(2);a();let n=_(4);p("ngTemplateOutlet",n)("ngTemplateOutletContext",E(2,z,t))}}function Rt(e,s){}function Ft(e,s){if(e&1&&T(0,"div",14),e&2){let t=a(2);p("ngClass",t.cls)("ngStyle",E(2,Tt,t.lines))}}function Nt(e,s){if(e&1&&m(0,Rt,0,0,"ng-template",13)(1,Ft,1,4,"ng-template",null,4,d),e&2){let t=_(2);a();let n=_(4);p("ngTemplateOutlet",n)("ngTemplateOutletContext",E(2,z,t))}}function Dt(e,s){}function kt(e,s){if(e&1&&M(0),e&2){let t=a(2);L(t.linsWord)}}function zt(e,s){if(e&1&&(x(0,"div",9)(1,"div",15),m(2,Dt,0,0,"ng-template",13)(3,kt,1,1,"ng-template",null,5,d),T(5,"div",16,6),x(7,"div",17,7)(9,"span"),M(10),w()()()()),e&2){let t=_(4),n=a(),i=_(4);p("ngClass",n.cls),C(2),p("ngTemplateOutlet",i)("ngTemplateOutletContext",E(5,z,t)),C(3),p("innerHTML",n.orgHtml,R),C(5),L(n.text)}}var Jt=(()=>{class e{constructor(){this.el=g(B).nativeElement,this.ngZone=g($),this.dom=g(st),this.doc=g(et),this.cdr=g(tt),this.isSupportLineClamp=this.doc.body.style.webkitLineClamp!==void 0,this.inited=!1,this.type="default",this.cls={},this.text="",this.targetCount=0,this.tooltip=!1,this.fullWidthRecognition=!1,this.tail="..."}get linsWord(){let{targetCount:t,text:n,tail:i}=this;return(t>0?n.substring(0,t):"")+(t>0&&t<n.length?i:"")}get win(){return this.doc.defaultView||window}getStrFullLength(t){return t.split("").reduce((n,i)=>{let l=i.charCodeAt(0);return l>=0&&l<=128?n+1:n+2},0)}cutStrByFullLength(t,n){let i=0;return t.split("").reduce((l,r)=>{let o=r.charCodeAt(0);return o>=0&&o<=128?i+=1:i+=2,i<=n?l+r:l},"")}bisection(t,n,i,l,r,o){let f=this.tail;o.innerHTML=r.substring(0,n)+f;let u=o.offsetHeight;return u<=t?(o.innerHTML=r.substring(0,n+1)+f,u=o.offsetHeight,u>t||n===i?n:(i=n,n=l-i===1?i+1:Math.floor((l-i)/2)+i,this.bisection(t,n,i,l,r,o))):n-1<0?n:(o.innerHTML=r.substring(0,n-1)+f,u=o.offsetHeight,u<=t?n-1:(l=n,n=Math.floor((l-i)/2)+i,this.bisection(t,n,i,l,r,o)))}genType(){let{lines:t,length:n,isSupportLineClamp:i}=this;this.cls={ellipsis:!0,ellipsis__lines:t&&!i,"ellipsis__line-clamp":t&&i},!t&&!n?this.type="default":t?i?this.type="line-clamp":this.type="line":this.type="length"}gen(){let{type:t,lines:n,length:i,fullWidthRecognition:l,tail:r,orgEl:o,cdr:f,ngZone:u}=this;if(t==="length"){let y=o.nativeElement;if(y.children.length>0)throw new Error("Ellipsis content must be string.");let h=y.textContent;if((l?this.getStrFullLength(h):h.length)<=i||i<0)this.text=h;else{let c;i-r.length<=0?c="":c=l?this.cutStrByFullLength(h,i):h.slice(0,i),this.text=c+r}u.run(()=>f.detectChanges())}else if(t==="line"){let{shadowOrgEl:y,shadowTextEl:h}=this,v=y.nativeElement,c=v.innerText||v.textContent,ut=parseInt(this.win.getComputedStyle(this.getEl(".ellipsis")).lineHeight,10),H=n*ut,mt=this.getEl(".ellipsis__handle");if(mt.style.height=`${H}px`,v.offsetHeight<=H)this.text=c,this.targetCount=c.length;else{let I=c.length,_t=Math.ceil(I/2),ht=this.bisection(H,_t,0,I,c,h.nativeElement.firstChild);this.text=c,this.targetCount=ht}u.run(()=>f.detectChanges())}}getEl(t){return this.el.querySelector(t)}executeOnStable(t){this.ngZone.isStable?t():this.ngZone.onStable.asObservable().pipe(Q(1)).subscribe(t)}refresh(){this.genType();let{type:t,dom:n,orgEl:i,cdr:l}=this,r=i.nativeElement.innerHTML;this.orgHtml=n.bypassSecurityTrustHtml(r),l.detectChanges(),this.executeOnStable(()=>{if(this.gen(),t!=="line"){let o=this.getEl(".ellipsis");o&&(o.innerHTML=r)}})}ngAfterViewInit(){this.inited=!0,this.refresh()}ngOnChanges(){this.inited&&this.refresh()}static{this.\u0275fac=function(n){return new(n||e)}}static{this.\u0275cmp=W({type:e,selectors:[["ellipsis"]],viewQuery:function(n,i){if(n&1&&(O(ft,5),O(gt,5),O(Ct,5)),n&2){let l;S(l=b())&&(i.orgEl=l.first),S(l=b())&&(i.shadowOrgEl=l.first),S(l=b())&&(i.shadowTextEl=l.first)}},inputs:{tooltip:[2,"tooltip","tooltip",D],length:[2,"length","length",t=>t==null?null:k(t)],lines:[2,"lines","lines",t=>t==null?null:k(t)],fullWidthRecognition:[2,"fullWidthRecognition","fullWidthRecognition",D],tail:"tail"},exportAs:["ellipsis"],standalone:!0,features:[P,j,X],ngContentSelectors:dt,decls:9,vars:1,consts:[["orgEl",""],["tooltipTpl",""],["titleTpl",""],["lengthTpl",""],["lineClampTpl",""],["lineTpl",""],["shadowOrgEl",""],["shadowTextEl",""],[2,"display","none",3,"cdkObserveContent"],[3,"ngClass"],["nz-tooltip","",3,"nzTooltipTitle","nzTooltipOverlayStyle"],[4,"ngTemplateOutlet"],[3,"innerHTML"],[3,"ngTemplateOutlet","ngTemplateOutletContext"],[3,"ngClass","ngStyle"],[1,"ellipsis__handle"],[1,"ellipsis__shadow",3,"innerHTML"],[1,"ellipsis__shadow"]],template:function(n,i){if(n&1){let l=U();J(),x(0,"div",8,0),G("cdkObserveContent",function(){return Z(l),q(i.refresh())}),K(2),w(),m(3,St,2,1,"ng-template",null,1,d)(5,bt,1,1,"span",9)(6,Ht,3,4)(7,Nt,3,4)(8,zt,11,7,"div",9)}if(n&2){let l;C(5),F((l=i.type)==="default"?5:l==="length"?6:l==="line-clamp"?7:l==="line"?8:-1)}},dependencies:[rt,at,lt,nt,it],encapsulation:2,changeDetection:0})}}return e})();var Kt=(()=>{class e{static{this.\u0275fac=function(n){return new(n||e)}}static{this.\u0275mod=A({type:e})}static{this.\u0275inj=V({imports:[ot,pt,ct]})}}return e})();export{Jt as a,Kt as b};