(function ( $ ) {
	$.fn.bdj_Accordion = function( options ) {		
		if (this.length > 1){
			this.each(function() { 
				$(this).bdj_Accordion(options);
			});
			return this;
		}
		var settings = $.extend({
			animation:  true,
			showIcon:   true,
			closeAble:  false,
			closeOther: true,
			slideSpeed: 150,
			activeIndex: false
		}, options );

		if( $(this).data('close-able') )    settings.closeAble = $(this).data('close-able');
		if( $(this).data('animation') )     settings.animation = $(this).data('animation');
		if( $(this).data('show-icon') )     settings.showIcon = $(this).data('show-icon');
		if( $(this).data('close-other') )   settings.closeOther = $(this).data('close-other');
		if( $(this).data('slide-speed') )   settings.slideSpeed = $(this).data('slide-speed');
		if( $(this).data('active-index') )  settings.activeIndex = $(this).data('active-index');
		var plugin = this;
		var init = function() {
			plugin.createStructure();
			plugin.clickHead();
		}
		this.createStructure = function() {
			plugin.addClass('bdj_accordion');
			if( settings.showIcon ){
				plugin.addClass('acc_with_icon');
			}
			if( plugin.find('.accordion_in').length < 1 ){
				plugin.children().addClass('accordion_in');
			}
			plugin.find('.accordion_in').each(function(index, elem){
				var childs = $(elem).children();
				$(childs[0]).addClass('acc_head');
				$(childs[1]).addClass('acc_content');
			});
			if( settings.showIcon ){
				plugin.find('.acc_head').prepend('<div class="acc_icon_expand"></div>');
			}
			plugin.find('.accordion_in .acc_content').not('.acc_active .acc_content').hide();

			if( settings.activeIndex === parseInt(settings.activeIndex) ){
				if(settings.activeIndex === 0){
					plugin.find('.accordion_in').addClass('acc_active').show();
					plugin.find('.accordion_in .acc_content').addClass('acc_active').show();
				}
				else{
					plugin.find('.accordion_in').eq(settings.activeIndex - 1).addClass('acc_active').show();
					plugin.find('.accordion_in .acc_content').eq(settings.activeIndex - 1).addClass('acc_active').show();
				}
			}
			
		}
		this.clickHead = function() {
			plugin.on('click', '.acc_head', function(){				
				var s_parent = $(this).parent();				
				if( s_parent.hasClass('acc_active') == false ){
					if( settings.closeOther ){
						plugin.find('.acc_content').slideUp(settings.slideSpeed);
						plugin.find('.accordion_in').removeClass('acc_active');
					}	
				}
				if( s_parent.hasClass('acc_active') ){
					if( false !== settings.closeAble ){
						s_parent.children('.acc_content').slideUp(settings.slideSpeed);
						s_parent.removeClass('acc_active');
					}
				}
				else{
					$(this).next('.acc_content').slideDown(settings.slideSpeed);
					s_parent.addClass('acc_active');
				}
			});
		}
		init();
		return this;

	};
}( jQuery ));