FROM node:20-alpine AS deps
WORKDIR /app
RUN apk add libc6-compat
RUN corepack enable
COPY package.json yarn.lock .yarnrc.yml ./
RUN yarn install --immutable

#Stage 2
FROM node:20-alpine AS builder
WORKDIR /app

RUN corepack enable

ARG NEXT_PUBLIC_ENABLE_IDEM_FORM=false
ARG NEXT_PUBLIC_SHOW_SEEDED_IDEMS=false

ENV NEXT_PUBLIC_ENABLE_IDEM_FORM=$NEXT_PUBLIC_ENABLE_IDEM_FORM
ENV NEXT_PUBLIC_SHOW_SEEDED_IDEMS=$NEXT_PUBLIC_SHOW_SEEDED_IDEMS

COPY --from=deps /app/.yarn ./.yarn
COPY --from=deps /app/package.json ./package.json
COPY --from=deps /app/yarn.lock ./yarn.lock
COPY --from=deps /app/.yarnrc.yml ./.yarnrc.yml
COPY --from=deps /app/node_modules ./node_modules

COPY . .

RUN yarn build

#Stage 3
FROM node:20-alpine AS runner
WORKDIR /app

ENV NODE_ENV=production
ENV NEXT_TELEMETRY_DISABLED=1

RUN addgroup --gid 1001 -S nodejs
RUN adduser -S nextjs -u 1001 -G nodejs

COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static
COPY --from=builder /app/public ./public

USER nextjs

ENV PORT=3000
ENV HOSTNAME="0.0.0.0"

EXPOSE 3000

CMD ["node", "server.js"]

