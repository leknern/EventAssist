<template>
	<q-list padding>
		<template v-for="(menuItem, index) in props.items" :key="index">
			<q-item clickable :to="menuItem.cb ? undefined : menuItem.url" v-ripple @click="menuItem.cb?.()">
				<q-item-section avatar>
					<q-icon :name="menuItem.icon" />
				</q-item-section>
				<q-item-section>
					{{ menuItem.label }}
				</q-item-section>
			</q-item>
			<q-separator v-if="menuItem.separator"></q-separator>
		</template>
	</q-list>
</template>

<script setup lang="ts">
import type { SidebarLink } from 'src/models/SidebarLink';

const props = defineProps<{ items: SidebarLink[] }>();
</script>
