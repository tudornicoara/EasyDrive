<script lang="ts">
    import {onMount} from "svelte";
    import type {User} from "../models/user";
    import agent from "../api/agent";

    let users: User[] = [];

    onMount(async () => {
        users = await agent.Users.list() as User[];
    })
</script>

{#if users.length === 0}
    <h2>Loading...</h2>
{:else}
    <ul>
        {#each users as user}
            <li>{user.name}</li>
        {/each}
    </ul>
{/if}