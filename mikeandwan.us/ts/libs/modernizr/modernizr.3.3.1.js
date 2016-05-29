/*! modernizr 3.3.1 (Custom Build) | MIT *
 * http://modernizr.com/download/?-audio-cssfilters-webgl-setclasses !*/
!function(e,n,t){function o(e,n){return typeof e===n}function r(){var e,n,t,r,s,a,i;for(var l in C)if(C.hasOwnProperty(l)){if(e=[],n=C[l],n.name&&(e.push(n.name.toLowerCase()),n.options&&n.options.aliases&&n.options.aliases.length))for(t=0;t<n.options.aliases.length;t++)e.push(n.options.aliases[t].toLowerCase());for(r=o(n.fn,"function")?n.fn():n.fn,s=0;s<e.length;s++)a=e[s],i=a.split("."),1===i.length?Modernizr[i[0]]=r:(!Modernizr[i[0]]||Modernizr[i[0]]instanceof Boolean||(Modernizr[i[0]]=new Boolean(Modernizr[i[0]])),Modernizr[i[0]][i[1]]=r),h.push((r?"":"no-")+i.join("-"))}}function s(e){var n=b.className,t=Modernizr._config.classPrefix||"";if(x&&(n=n.baseVal),Modernizr._config.enableJSClass){var o=new RegExp("(^|\\s)"+t+"no-js(\\s|$)");n=n.replace(o,"$1"+t+"js$2")}Modernizr._config.enableClasses&&(n+=" "+t+e.join(" "+t),x?b.className.baseVal=n:b.className=n)}function a(){return"function"!=typeof n.createElement?n.createElement(arguments[0]):x?n.createElementNS.call(n,"http://www.w3.org/2000/svg",arguments[0]):n.createElement.apply(n,arguments)}function i(e,n){return!!~(""+e).indexOf(n)}function l(e,n){return function(){return e.apply(n,arguments)}}function u(e,n,t){var r;for(var s in e)if(e[s]in n)return t===!1?e[s]:(r=n[e[s]],o(r,"function")?l(r,t||n):r);return!1}function p(e){return e.replace(/([a-z])-([a-z])/g,function(e,n,t){return n+t.toUpperCase()}).replace(/^-/,"")}function f(e){return e.replace(/([A-Z])/g,function(e,n){return"-"+n.toLowerCase()}).replace(/^ms-/,"-ms-")}function c(){var e=n.body;return e||(e=a(x?"svg":"body"),e.fake=!0),e}function d(e,t,o,r){var s,i,l,u,p="modernizr",f=a("div"),d=c();if(parseInt(o,10))for(;o--;)l=a("div"),l.id=r?r[o]:p+(o+1),f.appendChild(l);return s=a("style"),s.type="text/css",s.id="s"+p,(d.fake?d:f).appendChild(s),d.appendChild(f),s.styleSheet?s.styleSheet.cssText=e:s.appendChild(n.createTextNode(e)),f.id=p,d.fake&&(d.style.background="",d.style.overflow="hidden",u=b.style.overflow,b.style.overflow="hidden",b.appendChild(d)),i=t(f,e),d.fake?(d.parentNode.removeChild(d),b.style.overflow=u,b.offsetHeight):f.parentNode.removeChild(f),!!i}function m(n,o){var r=n.length;if("CSS"in e&&"supports"in e.CSS){for(;r--;)if(e.CSS.supports(f(n[r]),o))return!0;return!1}if("CSSSupportsRule"in e){for(var s=[];r--;)s.push("("+f(n[r])+":"+o+")");return s=s.join(" or "),d("@supports ("+s+") { #modernizr { position: absolute; } }",function(e){return"absolute"==getComputedStyle(e,null).position})}return t}function y(e,n,r,s){function l(){f&&(delete j.style,delete j.modElem)}if(s=o(s,"undefined")?!1:s,!o(r,"undefined")){var u=m(e,r);if(!o(u,"undefined"))return u}for(var f,c,d,y,v,g=["modernizr","tspan"];!j.style;)f=!0,j.modElem=a(g.shift()),j.style=j.modElem.style;for(d=e.length,c=0;d>c;c++)if(y=e[c],v=j.style[y],i(y,"-")&&(y=p(y)),j.style[y]!==t){if(s||o(r,"undefined"))return l(),"pfx"==n?y:!0;try{j.style[y]=r}catch(h){}if(j.style[y]!=v)return l(),"pfx"==n?y:!0}return l(),!1}function v(e,n,t,r,s){var a=e.charAt(0).toUpperCase()+e.slice(1),i=(e+" "+z.join(a+" ")+a).split(" ");return o(n,"string")||o(n,"undefined")?y(i,n,r,s):(i=(e+" "+E.join(a+" ")+a).split(" "),u(i,n,t))}function g(e,n,o){return v(e,t,t,n,o)}var h=[],C=[],w={_version:"3.3.1",_config:{classPrefix:"",enableClasses:!0,enableJSClass:!0,usePrefixes:!0},_q:[],on:function(e,n){var t=this;setTimeout(function(){n(t[e])},0)},addTest:function(e,n,t){C.push({name:e,fn:n,options:t})},addAsyncTest:function(e){C.push({name:null,fn:e})}},Modernizr=function(){};Modernizr.prototype=w,Modernizr=new Modernizr;var b=n.documentElement,x="svg"===b.nodeName.toLowerCase();Modernizr.addTest("audio",function(){var e=a("audio"),n=!1;try{(n=!!e.canPlayType)&&(n=new Boolean(n),n.ogg=e.canPlayType('audio/ogg; codecs="vorbis"').replace(/^no$/,""),n.mp3=e.canPlayType('audio/mpeg; codecs="mp3"').replace(/^no$/,""),n.opus=e.canPlayType('audio/ogg; codecs="opus"')||e.canPlayType('audio/webm; codecs="opus"').replace(/^no$/,""),n.wav=e.canPlayType('audio/wav; codecs="1"').replace(/^no$/,""),n.m4a=(e.canPlayType("audio/x-m4a;")||e.canPlayType("audio/aac;")).replace(/^no$/,""))}catch(t){}return n}),Modernizr.addTest("webgl",function(){var n=a("canvas"),t="probablySupportsContext"in n?"probablySupportsContext":"supportsContext";return t in n?n[t]("webgl")||n[t]("experimental-webgl"):"WebGLRenderingContext"in e});var S=w._config.usePrefixes?" -webkit- -moz- -o- -ms- ".split(" "):["",""];w._prefixes=S;var T="CSS"in e&&"supports"in e.CSS,_="supportsCSS"in e;Modernizr.addTest("supports",T||_);var P="Moz O ms Webkit",z=w._config.usePrefixes?P.split(" "):[];w._cssomPrefixes=z;var E=w._config.usePrefixes?P.toLowerCase().split(" "):[];w._domPrefixes=E;var N={elem:a("modernizr")};Modernizr._q.push(function(){delete N.elem});var j={style:N.elem.style};Modernizr._q.unshift(function(){delete j.style}),w.testAllProps=v,w.testAllProps=g,Modernizr.addTest("cssfilters",function(){if(Modernizr.supports)return g("filter","blur(2px)");var e=a("a");return e.style.cssText=S.join("filter:blur(2px); "),!!e.style.length&&(n.documentMode===t||n.documentMode>9)}),r(),s(h),delete w.addTest,delete w.addAsyncTest;for(var $=0;$<Modernizr._q.length;$++)Modernizr._q[$]();e.Modernizr=Modernizr}(window,document);