export interface SidebarLink {
	label: string;
	url: string;
	icon: string;
	separator: boolean;
	roles?: string[];
	cb?: () => void | Promise<void>;
}
