import{Z as v}from"./chunk-DTT4EXDO.js";import{$ as h,Ca as $,Ea as k,Fa as p,Ja as A,P as I,Pb as Z,Qb as H,Rb as d,V as x,W as w,Wc as G,Xc as Q,Z as s,aa as T,ba as F,bd as L,ca as O,d as m,ga as S,ic as W,j as y,la as M,lb as E,mb as z,nb as B,oa as j,pa as u,xb as N,y as C}from"./chunk-AYH25EOF.js";import{a as D}from"./chunk-MON7YFGF.js";var K=["container"],X=(()=>{let t=class t{get cog(){return this._cog}set cog(e){this._cog=this.cogSrv.merge("chart",{theme:"",libs:["https://gw.alipayobjects.com/os/lib/antv/g2/4.1.46/dist/g2.min.js","https://gw.alipayobjects.com/os/lib/antv/data-set/0.11.8/dist/data-set.js"]},e)}constructor(){this.cogSrv=s(Q),this.lazySrv=s(G),this.loading=!1,this.loaded=!1,this.notify$=new m,this.cog={theme:""}}libLoad(){return this.loading?(this.loaded&&this.notify$.next(),this):(this.loading=!0,this.lazySrv.load(this.cog.libs).then(()=>{this.loaded=!0,this.notify$.next()}),this)}get notify(){return this.notify$.asObservable()}ngOnDestroy(){this.notify$.unsubscribe()}};t.\u0275fac=function(o){return new(o||t)},t.\u0275prov=x({token:t,factory:t.\u0275fac,providedIn:"root"});let i=t;return i})(),P=(()=>{let t=class t{get chart(){return this._chart}get winG2(){return window.G2}constructor(){this.srv=s(X),this.el=s(j),this.ngZone=s($),this.platform=s(L),this.cdr=s(Z),this.repaint=!0,this.destroy$=new m,this.loaded=!1,this.delay=0,this.ready=new u,this.theme=this.srv.cog.theme,this.srv.notify.pipe(I(this.destroy$),C(()=>!this.loaded)).subscribe(()=>this.load())}changeData(){}onInit(){}onChanges(e){}load(){this.ngZone.run(()=>{this.loaded=!0,this.cdr.detectChanges()}),setTimeout(()=>this.install(),this.delay)}ngOnInit(){this.platform.isBrowser&&(this.onInit(),this.winG2?this.load():this.srv.libLoad())}ngOnChanges(e){if(this.onChanges(e),this.onlyChangeData?this.onlyChangeData(e):Object.keys(e).length===1&&!!e.data){this.changeData();return}!this.chart||!this.repaint||this.ngZone.runOutsideAngular(()=>{this.destroyChart().install()})}destroyChart(){return this._chart&&this._chart.destroy(),this}ngOnDestroy(){this.resize$&&this.resize$.unsubscribe(),this.destroy$.next(),this.destroy$.complete(),this.destroyChart()}};t.\u0275fac=function(o){return new(o||t)},t.\u0275dir=O({type:t,viewQuery:function(o,n){if(o&1&&E(K,7),o&2){let c;z(c=B())&&(n.node=c.first)}},inputs:{repaint:[h.HasDecoratorInputTransform,"repaint","repaint",H],delay:[h.HasDecoratorInputTransform,"delay","delay",d],theme:"theme"},outputs:{ready:"ready"},features:[p,S]});let i=t;return y([v()],i.prototype,"load",null),y([v()],i.prototype,"destroyChart",null),i})();function R(i,t){let l=D({showTitle:!1,showMarkers:!0,enterable:!0,domStyles:{"g2-tooltip":{padding:"0px"},"g2-tooltip-title":{display:"none"},"g2-tooltip-list-item":{margin:"4px"}}},t);return i==="mini"&&(l.position="top",l.domStyles["g2-tooltip"]={padding:"0px",backgroundColor:"transparent",boxShadow:"none"},l.itemTpl="<li>{value}</li>",l.offset=8),l}var It=(()=>{let t=class t extends P{constructor(){super(...arguments),this.color="#1890FF",this.height=0,this.borderWidth=5,this.padding=[8,8,8,8],this.data=[],this.yTooltipSuffix="",this.tooltipType="default",this.clickItem=new u}install(){let{el:e,height:o,padding:n,yTooltipSuffix:c,tooltipType:V,theme:q,color:U,borderWidth:_}=this,r=this._chart=new this.winG2.Chart({container:e.nativeElement,autoFit:!0,height:o,padding:n,theme:q});r.scale({x:{type:"cat"},y:{min:0}}),r.legend(!1),r.axis(!1),r.tooltip(R(V,{showCrosshairs:!1})),r.interval().position("x*y").color("x*y",(a,f)=>{let g=this.data.find(b=>b.x===a&&b.y===f);return g&&g.color?g.color:U}).size(_).tooltip("x*y",(a,f)=>({name:a,value:f+c})),r.on("interval:click",a=>{this.ngZone.run(()=>this.clickItem.emit({item:a.data?.data,ev:a}))}),this.ready.next(r),this.changeData(),r.render()}changeData(){let{_chart:e,data:o}=this;!e||!Array.isArray(o)||o.length<=0||e.changeData(o)}};t.\u0275fac=(()=>{let e;return function(n){return(e||(e=M(t)))(n||t)}})(),t.\u0275cmp=T({type:t,selectors:[["g2-mini-bar"]],hostVars:2,hostBindings:function(o,n){o&2&&A("height",n.height,"px")},inputs:{color:"color",height:[h.HasDecoratorInputTransform,"height","height",d],borderWidth:[h.HasDecoratorInputTransform,"borderWidth","borderWidth",d],padding:"padding",data:"data",yTooltipSuffix:"yTooltipSuffix",tooltipType:"tooltipType"},outputs:{clickItem:"clickItem"},exportAs:["g2MiniBar"],standalone:!0,features:[p,k,N],decls:0,vars:0,template:function(o,n){},encapsulation:2,changeDetection:0});let i=t;return i})();var xt=(()=>{let t=class t{};t.\u0275fac=function(o){return new(o||t)},t.\u0275mod=F({type:t}),t.\u0275inj=w({imports:[W]});let i=t;return i})();export{P as a,R as b,It as c,xt as d};
